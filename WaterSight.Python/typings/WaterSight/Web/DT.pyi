from enum import Enum
from typing import List

class DigitalTwinType(Enum):
	Water = 0
	Sewer = 1

class DigitalTwinExt:

	def __init__(self) -> None:
		"""Creating a new Instance of this class is not allowed


		Raises
		--------
			Exception: if this class is instantiated
		"""
		raise Exception("Creating a new Instance of this class is not allowed")
		pass

class DigitalTwinCreateOptions:

	def __init__(self, digitalTwinType: DigitalTwinType) -> None:
		"""Creating a new Instance of this class is not allowed


		Raises
		--------
			Exception: if this class is instantiated
		"""
		raise Exception("Creating a new Instance of this class is not allowed")
		pass

	@property
	def DigitalTwinName(self) -> str:
		"""No Description

		Returns
		--------
			``DigitalTwinCreateOptions`` : 
		"""
		pass

	@DigitalTwinName.setter
	def DigitalTwinName(self, digitaltwinname: str) -> None:
		pass

	@property
	def DigitalTwinType(self) -> DigitalTwinType:
		"""No Description

		Returns
		--------
			``DigitalTwinCreateOptions`` : 
		"""
		pass

	@DigitalTwinType.setter
	def DigitalTwinType(self, digitaltwintype: DigitalTwinType) -> None:
		pass

	@property
	def Goals(self) -> List[str]:
		"""No Description

		Returns
		--------
			``DigitalTwinCreateOptions`` : 
		"""
		pass

	@Goals.setter
	def Goals(self, goals: List[str]) -> None:
		pass

	@property
	def DigitalTwinTypeName(self) -> str:
		"""No Description

		Returns
		--------
			``DigitalTwinCreateOptions`` : 
		"""
		pass

	@DigitalTwinTypeName.setter
	def DigitalTwinTypeName(self, digitaltwintypename: str) -> None:
		pass

