class PositionCantBeReachedException(Exception):
    def __init__(self, msg: str = "", robot = None):
        if robot is not None:
            robot.set_colour(robot.RED)
        super().__init__(self, msg)


class InvalidArgumentException(Exception):
    def __init__(self, msg: str = ""):
        super().__init__(self, msg)