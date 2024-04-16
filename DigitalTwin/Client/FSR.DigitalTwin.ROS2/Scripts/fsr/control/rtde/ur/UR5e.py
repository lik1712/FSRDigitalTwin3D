import numpy as np
import time
from math import degrees, radians, pi
from rtde_control import RTDEControlInterface  # https://sdurobotics.gitlab.io/ur_rtde/api/api.html
from rtde_receive import RTDEReceiveInterface
from rtde_io import RTDEIOInterface
from scipy.spatial.transform import Rotation as R  # https://docs.scipy.org/doc/scipy/reference/generated/scipy.spatial.transform.Rotation.html
from fsr.exception import InvalidArgumentException, PositionCantBeReachedException
from fsr.utils.linalg import euler_deg2axis_angle, rotation_matrix, vector2euler_deg

class UR5e:
    # Robot host
    ROBOT_HOST = "192.168.0.100"
    ROBOT_PORT = 30004

    # Default speeds
    SDT_SPEED_LIN = 0.2  # in m/s
    STD_ACCELERATION_LIN = 0.5  # in m/s^2
    STD_SPEED_RAD = 1  # in rad/s
    STD_SPEED_DEG = degrees(STD_SPEED_RAD)  # in deg/s
    STD_ACCELERATION_RAD = 1  # in rad/s^2
    STD_ACCELERATION_DEG = degrees(STD_ACCELERATION_RAD)  # in deg/s^2

    Z_MIN = 0  # in m

    # constants for LED
    OFF = 0
    GREEN = 1
    YELLOW = 2
    RED = 3

    def __init__(self, ip: str = ROBOT_HOST, verbose: bool = False):
        """
        Connect to robot
        :param ip: ip address of robot as string (e.g. "192.168.0.100")
        :param verbose:
        """
        self.rtde_c = RTDEControlInterface(ip, RTDEControlInterface.FLAG_USE_EXT_UR_CAP)
        self.rtde_r = RTDEReceiveInterface(ip)
        self.rtde_io = RTDEIOInterface(ip, verbose=verbose)
        self.set_colour(self.GREEN)

    def get_joint_positions_rad(self) -> tuple:
        """
       Returns current joint positions in deg
       :return: joint positions of base, shoulder, elbow, wrist1, wrist2, wrist3 in rad
       """
        p = self.rtde_c.getActualJointPositionsHistory(0)
        return p[0], p[1], p[2], p[3], p[4], p[5]

    def get_joint_positions_deg(self) -> tuple:
        """
        Returns current joint positions in deg
        :return: joint positions of base, shoulder, elbow, wrist1, wrist2, wrist3 in deg
        """
        p = self.rtde_c.getActualJointPositionsHistory(0)
        return degrees(p[0]), degrees(p[1]), degrees(p[2]), degrees(p[3]), degrees(p[4]), degrees(p[5])

    def get_current_tcp_position(self) -> tuple:
        """
        Returns current tcp positions
        :return: x-coordinate ( = to(+) and away(-) from me) of tcp in mm,
                 y-coordinate ( = to(+) and away(-) from window) of tcp in mm,
                 z-coordinate = height of tcp in mm (0 = base level),
                 x-coordinate of tcp rotation in rad,
                 y-coordinate of tcp rotation in rad,
                 z-coordinate of tcp rotation in rad
                    as rotation vector
        """
        p = self.rtde_r.getActualTCPPose()
        return p[0] * 1000, p[1] * 1000, p[2] * 1000, p[3], p[4], p[5]

    def moveJ_rad(self, base: float, shoulder: float, elbow: float, wrist1: float, wrist2: float, wrist3: float,
                  speed: float = STD_SPEED_RAD, acceleration: float = STD_ACCELERATION_RAD) -> None:
        """
        Move to joint position
        :param base: target base-joint position in rad
        :param shoulder: target shoulder-joint position in rad
        :param elbow: target elbow-joint position in rad
        :param wrist1: target wrist1-joint position in rad
        :param wrist2: target wrist2-joint position in rad
        :param wrist3: target wrist3-joint position in rad
        :param speed: joint speed of leading axis in rad/s
        :param acceleration: joint acceleration of leading axis in rad/s^2
        :return: None
        """
        if speed <= 0.0 or acceleration <= 0.0:
            raise InvalidArgumentException("Speed and acceleration must be positive.")
        if self.rtde_c.isJointsWithinSafetyLimits([base, shoulder, elbow, wrist1, wrist2, wrist3]):
            self.set_freedrive(False)
            self.rtde_c.moveJ([base, shoulder, elbow, wrist1, wrist2, wrist3], speed, acceleration)
            if self.isProtectiveStopped():
                raise PositionCantBeReachedException("Robot went into protective stop while trying to reach position " +
                                                     "[base= " + str(base) + "; shoulder=" + str(shoulder) +
                                                     "; elbow=" + str(elbow) + "; wrist1=" + str(wrist1) + "; wrist2=" +
                                                     str(wrist2) + "; wrist3=" + str(wrist3) + "].")
        else:
            raise PositionCantBeReachedException("Robot will not be able to reach position [base=" + str(base) +
                                                 "; shoulder=" + str(shoulder) + "; elbow=" + str(elbow) + "; wrist1=" +
                                                 str(wrist1) + "; wrist2=" + str(wrist2) + "; wrist3=" + str(wrist3) +
                                                 "].")

    def moveJ_deg(self, base: float, shoulder: float, elbow: float, wrist1: float, wrist2: float, wrist3: float,
                  speed: float = STD_SPEED_DEG, acceleration: float = STD_ACCELERATION_DEG) -> None:
        """
        Move to joint position
        :param base: target base-joint position in deg
        :param shoulder: target shoulder-joint position in deg
        :param elbow: target elbow-joint position in deg
        :param wrist1: target wrist1-joint position in deg
        :param wrist2: target wrist2-joint position in deg
        :param wrist3: target wrist3-joint position in deg
        :param speed: joint speed of leading axis in deg/s
        :param acceleration: joint acceleration of leading axis in deg/s^2
        :return: None
        """
        self.moveJ_rad(radians(base), radians(shoulder), radians(elbow), radians(wrist1), radians(wrist2),
                       radians(wrist3), radians(speed), radians(acceleration))

    def moveL(self, x: float, y: float, z: float, rx: float, ry: float, rz: float, speed: float = SDT_SPEED_LIN,
              acceleration: float = STD_ACCELERATION_LIN) -> None:
        """
        Move to position (linear in tool-space)
        :param x: x-coordinate ( = to(+) and away(-) from me) of target pose in mm
        :param y: y-coordinate ( = to(+) and away(-) from window) of target pose in mm
        :param z: z-coordinate = height of target pose in mm (0 = base level)
        :param rx: x-coordinate of target rotation in deg
        :param ry: y-coordinate of target rotation in deg
        :param rz: z-coordinate of target rotation in deg
            as euler angles in (extrinsic) XYZ
        :param speed: tool speed in m/s
        :param acceleration: tool acceleration in m/s^2
        :return: None
        """
        if speed <= 0.0 or acceleration <= 0.0:
            raise InvalidArgumentException("Speed and acceleration must be positive.")
        x /= 1000
        y /= 1000
        z /= 1000
        rx, ry, rz = euler_deg2axis_angle(rx, ry, rz)
        if self.rtde_c.isPoseWithinSafetyLimits([x, y, z, rx, ry, rz]) and z >= self.Z_MIN:
            self.set_freedrive(False)
            self.rtde_c.moveL([x, y, z, rx, ry, rz], speed, acceleration)
            if self.isProtectiveStopped():
                raise PositionCantBeReachedException("Robot went into protective stop while trying to reach position "
                                                     "[x=" + str(x) + "; y=" + str(y) + "; z=" + str(z) + "; rx=" +
                                                     str(rx) + "; ry=" + str(ry) + "; rz=" + str(rz) + "].")
        else:
            raise PositionCantBeReachedException("Robot will not be able to reach position [x=" + str(x) + "; y=" +
                                                 str(y) + "; z=" + str(z) + "; rx=" + str(rx) + "; ry=" + str(ry) +
                                                 "; rz=" + str(rz) + "].")

    def move(self, x: float, y: float, z: float, rx: float, ry: float, rz: float, speed: float = STD_SPEED_RAD,
             acceleration: float = STD_ACCELERATION_RAD, max_position_error: float = 1e-10,
             max_orientation_error: float = 1e-10, closest_to_joints_zero_positions: bool = False) -> None:
        """
        Moves to given position (does not move linear)
        :param x: x-coordinate ( = to(+) and away(-) from me) of target pose in mm
        :param y: y-coordinate ( = to(+) and away(-) from window) of target pose in mm
        :param z: z-coordinate = height of target pose in mm (0 = base level)
        :param rx: x-coordinate = height of target pose in deg
        :param ry: y-coordinate = height of target pose in deg
        :param rz: z-coordinate = height of target pose in deg
            as euler angles in (extrinsic) XYZ
         :param speed: tool speed in m/s
        :param acceleration: tool acceleration in m/s^2
        :param max_position_error: the maximum allowed position error
        :param max_orientation_error: the maximum allowed orientation error
        :param closest_to_joints_zero_positions: if true, no joint will be turned more than 180 deg from its zero
            position
        :return: None
        """
        print("MOVING TO", x, y, z, rx, ry, rz, "in mm")
        base, shoulder, elbow, wrist1, wrist2, wrist3 = \
            self.getInverseKinematics(x, y, z, rx, ry, rz, max_position_error, max_orientation_error)
        if closest_to_joints_zero_positions:
            if wrist1 > pi:
                wrist1 -= pi * 2
            elif wrist1 < -pi:
                wrist1 += pi * 2
            if wrist2 > pi:
                wrist2 -= pi * 2
            elif wrist2 < -pi:
                wrist2 += pi * 2
            if wrist3 > pi:
                wrist3 -= pi * 2
            elif wrist3 < -pi:
                wrist3 += pi * 2
        self.moveJ_rad(base, shoulder, elbow, wrist1, wrist2, wrist3, speed, acceleration)

    def _turn_tcp(self, rot_angle: float):  # rot_angle in deg
        # todo: unnecessary, just rotate wrist 3?
        # fixme
        x, y, z, rx, ry, rz = self.get_current_tcp_position()
        r1 = R.from_rotvec((rx, ry, rz)).as_matrix()
        r2 = rotation_matrix([0, 0, 1], radians(rot_angle))
        r12 = np.dot(r1, r2)
        rx, ry, rz = R.from_matrix(r12).as_euler("xyz", degrees=True)
        self.moveL(x, y, z, rx, ry, rz)

    def home(self, speed: float = STD_SPEED_DEG, acceleration: float = STD_ACCELERATION_DEG) -> None:
        """
        Move to home position
        :param speed: joint speed of leading axis in deg/s
        :param acceleration: joint acceleration of leading axis in deg/s^2
        :return: None
        """
        self.moveJ_deg(-90, -90, -90, -90, 90, 0, speed, acceleration)

    def workspace(self, speed: float = STD_SPEED_DEG, acceleration: float = STD_ACCELERATION_DEG) -> None:
        """
        Move to workspace default position
        :param speed: joint speed of leading axis in deg/s
        :param acceleration: joint acceleration of leading axis in deg/s^2
        :return: None
        """
        self.moveJ_deg(-45, -120, -90, -60, 90, 0, speed, acceleration)

    def set_freedrive(self, freedrive: bool) -> None:
        """
        Start or stop freedrive-mode. True = start freedrive, false = stop freedrive
        :param freedrive:
        :return: None
        """
        if freedrive:
            self.rtde_c.teachMode()
        else:
            self.rtde_c.endTeachMode()

    def trigger_protective_stop(self) -> None:
        """
        Triggers a protective stop on the robot.
        :return: None
        """
        self.rtde_c.triggerProtectiveStop()

    def isEmergencyStopped(self) -> bool:
        """
        Returns a bool indicating if the robot is in ‘Emergency stop’
        :return: bool indicating if the robot is in ‘Emergency stop’
        """
        return self.rtde_r.isEmergencyStopped()

    def isProtectiveStopped(self) -> bool:
        """
        Returns a bool indicating if the robot is in ‘Protective stop’
        :return: bool indicating if the robot is in ‘Protective stop’
        """
        return self.rtde_r.isProtectiveStopped()

    def set_speed_slider(self, speed: int) -> None:
        """
        Set the speed slider on the controller
        :param speed: set the speed slider on the controller as percentage value between 0 and 100
        :return:
        """
        speed = max(0, min(100, speed))
        self.rtde_io.setSpeedSlider(speed / 100)

    def is_gripper_closed(self) -> bool:
        """
        Returns true if gripper is closed, false if gripper is open
        :return: true if gripper is closed, false if gripper is open
        """
        p = self.rtde_r.getActualDigitalInputBits()
        p = (p & (1 << 3)) >> 3  # looks at bit 4
        return p == 1

    def close_gripper(self, timeout: float = 0) -> None:
        """
        Closes gripper and waits until gripper is closed. If gripper takes more time to close than is set in timeout,
        a PositionCantBeReachedException will be raised. If timeout is zero, methode doesn't wait for gripper to close.
        :param timeout: time before PositionCantBeReachedException is raised in seconds
        :return: None
        """
        start = time.time()
        self.rtde_io.setStandardDigitalOut(1, False)
        self.rtde_io.setStandardDigitalOut(0, True)
        if timeout == 0:
            return
        while not self.is_gripper_closed():
            if time.time() - start > timeout:
                raise PositionCantBeReachedException("Gripper didn't close in " + str(timeout) + "seconds.")

    def open_gripper(self, timeout: float = 0) -> None:
        """
        Opens gripper and waits until gripper is opened. If gripper takes more time to open than is set in timeout,
        a PositionCantBeReachedException will be raised. If timeout is zero, methode doesn't wait for gripper to open.
        :param timeout: time before PositionCantBeReachedException is raised in seconds
        :return: None
        """
        start = time.time()
        self.rtde_io.setStandardDigitalOut(0, False)
        self.rtde_io.setStandardDigitalOut(1, True)
        if timeout == 0:
            return
        while self.is_gripper_closed():
            if time.time() - start > timeout:
                raise PositionCantBeReachedException("Gripper didn't open in " + str(timeout) + "seconds.")

    def set_colour(self, colour: int = OFF) -> None:
        """
        Lights uo the gripper in given colour
        :param colour: colour from constants OFF, GREEN, YELLOW or RED
        :return: None
        """
        if colour == self.OFF:
            self.rtde_io.setStandardDigitalOut(2, False)
            self.rtde_io.setStandardDigitalOut(3, False)
        elif colour == self.YELLOW:
            self.rtde_io.setStandardDigitalOut(2, True)
            self.rtde_io.setStandardDigitalOut(3, False)
        elif colour == self.GREEN:
            self.rtde_io.setStandardDigitalOut(2, False)
            self.rtde_io.setStandardDigitalOut(3, True)
        elif colour == self.RED:
            self.rtde_io.setStandardDigitalOut(2, True)
            self.rtde_io.setStandardDigitalOut(3, True)

    def getInverseKinematics(self, x: float, y: float, z: float, rx: float, ry: float, rz: float,
                             max_position_error: float = 1e-10, max_orientation_error: float = 1e-10) -> tuple:
        """
        Calculates joint positions to reach given point
        :param x: x-coordinate ( = to(+) and away(-) from me) of target pose in mm
        :param y: y-coordinate ( = to(+) and away(-) from window) of target pose in mm
        :param z: z-coordinate = height of target pose in mm (0 = base level)
        :param rx: x-coordinate = height of target pose in deg
        :param ry: y-coordinate = height of target pose in deg
        :param rz: z-coordinate = height of target pose in deg
            as euler angles in (extrinsic) XYZ
        :param max_position_error: the maximum allowed position error
        :param max_orientation_error: the maximum allowed orientation error
        :return: positions of joints base, shoulder, elbow, wrist1, wrist2, wrist3 in rad
        """
        x /= 1000
        y /= 1000
        z /= 1000
        rx, ry, rz = euler_deg2axis_angle(rx, ry, rz)
        if not self.rtde_c.isPoseWithinSafetyLimits((x, y, z, rx, ry, rz)):
            raise PositionCantBeReachedException("Robot will not be able to reach position [x=" + str(x) + "; y=" +
                                                 str(y) + "; z=" + str(z) + "; rx=" + str(rx) + "; ry=" + str(ry) +
                                                 "; rz=" + str(rz) + "] in m.")
        try:
            base, shoulder, elbow, wrist1, wrist2, wrist3 = \
                self.rtde_c.getInverseKinematics([x, y, z, rx, ry, rz], max_position_error=max_position_error,
                                                 max_orientation_error=max_orientation_error)
        except ValueError:
            raise PositionCantBeReachedException("Script function get_inverse_kin us unable to find an inverse "
                                                 "kinematic solution.")
        return base, shoulder, elbow, wrist1, wrist2, wrist3

    def pick(self, x: float, y: float, z: float, vx: float, vy: float, vz: float, dist: float = 75,
             speed_lin: float = SDT_SPEED_LIN, acceleration_lin: float = STD_ACCELERATION_LIN,
             speed_rad: float = STD_SPEED_RAD, acceleration_rad: float = STD_ACCELERATION_RAD,
             closest_to_joints_zero_positions: bool = False, delay: float = 0.5,
             max_position_error: float = 1e-10, max_orientation_error: float = 1e-10, ):
        dv = [vx, vy, vz]
        dv /= np.linalg.norm(dv)
        dv = np.dot(dv, -dist)
        p1 = np.add([x, y, z], dv)
        rx, ry, rz = vector2euler_deg(vx, vy, vz)
        self.move(p1[0], p1[1], p1[2], rx, ry, rz, speed_rad, acceleration_rad, max_position_error,
                  max_orientation_error, closest_to_joints_zero_positions)
        self.open_gripper(1)
        time.sleep(delay)
        self.moveL(x, y, z, rx, ry, rz, speed_lin, acceleration_lin)
        self.close_gripper()
        time.sleep(delay)
        self.moveL(p1[0], p1[1], p1[2], rx, ry, rz, speed_lin, acceleration_lin)

    def place(self, x: float, y: float, z: float, vx: float, vy: float, vz: float, dist: float = 75,
            speed_lin: float = SDT_SPEED_LIN, acceleration_lin: float = STD_ACCELERATION_LIN,
            speed_rad: float = STD_SPEED_RAD, acceleration_rad: float = STD_ACCELERATION_RAD,
            closest_to_joints_zero_positions: bool = False, delay: float = 0.5,
            max_position_error: float = 1e-10, max_orientation_error: float = 1e-10, ):
        dv = [vx, vy, vz]
        dv /= np.linalg.norm(dv)
        dv = np.dot(dv, -dist)
        p1 = np.add([x, y, z], dv)
        rx, ry, rz = vector2euler_deg(vx, vy, vz)
        self.move(p1[0], p1[1], p1[2], rx, ry, rz, speed_rad, acceleration_rad, max_position_error,
                  max_orientation_error, closest_to_joints_zero_positions)
        time.sleep(delay)
        self.moveL(x, y, z, rx, ry, rz, speed_lin, acceleration_lin)
        self.open_gripper(1)
        time.sleep(delay)
        self.moveL(p1[0], p1[1], p1[2], rx, ry, rz, speed_lin, acceleration_lin)


