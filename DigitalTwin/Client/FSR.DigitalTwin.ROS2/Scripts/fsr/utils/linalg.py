import numpy as np
from scipy.spatial.transform import Rotation as R  # https://docs.scipy.org/doc/scipy/reference/generated/scipy.spatial.transform.Rotation.html
from math import degrees, radians, pi, isnan, sin, cos, sqrt

def euler_deg2axis_angle(rx: float, ry: float, rz: float):
    """
    Convert euler angles in (extrinsic) XYZ in deg to axis angle in rad
    :param rx: rx in deg
    :param ry: ry in deg
    :param rz: rz in deg
    :return: (rx, ry, rz) in rad
    """
    rot = R.from_euler("xyz", [rx, ry, rz], degrees=True)
    return rot.as_rotvec()


def euler_rad2axis_angle(rx: float, ry: float, rz: float):
    """
    Convert euler angles in (extrinsic) XYZ in rad to axis angle in rad
    :param rx: rx in rad
    :param ry: ry in rad
    :param rz: rz in rad
    :return: (rx, ry, rz) in rad
    """
    rot = R.from_euler("xyz", [rx, ry, rz], degrees=False)
    return rot.as_rotvec()


def rotation_matrix_from_vectors(vec1, vec2):
    """ Find the rotation matrix that aligns vec1 to vec2
    from https://stackoverflow.com/questions/45142959/calculate-rotation-matrix-to-align-two-vectors-in-3d-space
    :param vec1: A 3d "source" vector
    :param vec2: A 3d "destination" vector
    :return mat: A transform matrix (3x3) which when applied to vec1, aligns it with vec2.
    """
    a, b = (vec1 / np.linalg.norm(vec1)).reshape(3), (vec2 / np.linalg.norm(vec2)).reshape(3)
    v = np.cross(a, b)
    c = np.dot(a, b)
    s = np.linalg.norm(v)
    kmat = np.array([[0, -v[2], v[1]], [v[2], 0, -v[0]], [-v[1], v[0], 0]])
    rot_matrix = np.eye(3) + kmat + kmat.dot(kmat) * ((1 - c) / (s ** 2))
    return rot_matrix


def rotation_matrix(axis, theta):
    """
    Return the rotation matrix associated with counterclockwise rotation about
    the given axis by theta radians.
    from: https://stackoverflow.com/questions/6802577/rotation-of-3d-vector
    """
    axis = np.asarray(axis)
    axis /= sqrt(np.dot(axis, axis))
    a = cos(theta / 2.0)
    b, c, d = -axis * sin(theta / 2.0)
    aa, bb, cc, dd = a * a, b * b, c * c, d * d
    bc, ad, ac, ab, bd, cd = b * c, a * d, a * c, a * b, b * d, c * d
    return np.array([[aa + bb - cc - dd, 2 * (bc + ad), 2 * (bd - ac)],
                     [2 * (bc - ad), aa + cc - bb - dd, 2 * (cd + ab)],
                     [2 * (bd + ac), 2 * (cd - ab), aa + dd - bb - cc]])


def vector2euler_deg(vx: float, vy: float, vz: float):
    # todo: gimbal locks
    v = [vx, vy, vz]
    v /= np.linalg.norm(v)
    if v[0] == 0 and v[1] == 0 and v[2] == -1:  # gimbal lock
        return -180, 0, 0

    up = [0, 0, 1]
    # noinspection PyTypeChecker
    r = R.from_matrix(rotation_matrix_from_vectors(up, v))
    rx, ry, rz = r.as_euler("xyz", degrees=True)
    if isnan(rx):
        rx = 0
    if isnan(ry):
        ry = 0
    if isnan(rz):
        rz = 0
    print(rx, ry, rz)
    return rx, ry, rz


def vectors2euler_deg(x, z):
    # fixme
    if not np.inner(x, z) == 0:
        raise InvalidArgumentException()
    y = np.cross(x, z)
    print(np.column_stack((x, y, z)))
    rot = R.from_matrix(np.column_stack((x, y, z)))
    return rot.as_euler("xyz", degrees=True)


def find_nearest(array, value):
    """
    Finds the item in array that is closest to value
    :param array: array of items to search in
    :param value:
    :return: item in array that is closest to value
    """
    array = np.asarray(array)
    idx = (np.abs(array - value)).argmin()
    return array[idx]