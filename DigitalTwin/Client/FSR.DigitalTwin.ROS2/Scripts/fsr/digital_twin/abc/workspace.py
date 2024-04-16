from abc import ABC, abstractmethod
from enum import Enum

class WorkspaceKind(Enum):
    DIGITAL = 0
    VIRTUAL = 1
    PHYSICAL = 2

class Workspace(ABC):
    @abstractmethod
    def _get_kind(self):
        pass

    @property
    def kind(self):
        return self._get_kind()