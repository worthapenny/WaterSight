from typing import Dict, List

class Util:

	def __init__(self) -> None:
		"""Creating a new Instance of this class is not allowed


		Raises
		--------
			Exception: if this class is instantiated
		"""
		raise Exception("Creating a new Instance of this class is not allowed")
		pass

	@staticmethod
	def StartTimer() -> Stopwatch:
		"""No Description

		Returns
		--------
			``Stopwatch`` : 
		"""
		pass

	@staticmethod
	def Elapsed(sw: Stopwatch) -> TimeSpan:
		"""No Description

		Args
		--------
			sw (``Stopwatch``) :  sw

		Returns
		--------
			``TimeSpan`` : 
		"""
		pass

	@staticmethod
	def ReadCsvToDataFrameAsync(filePath: str, dataTypeMap: Dict[str,str][str,Type]) -> Task:
		"""No Description

		Args
		--------
			filePath (``str``) :  filePath
			dataTypeMap (``Dict[str,str]``) :  dataTypeMap

		Returns
		--------
			``Task`` : 
		"""
		pass

	@staticmethod
	def GetCsvHeader(filePath: str) -> List[str]:
		"""No Description

		Args
		--------
			filePath (``str``) :  filePath

		Returns
		--------
			``List[str]`` : 
		"""
		pass

	@staticmethod
	def IsAdministrator() -> bool:
		"""No Description

		Returns
		--------
			``bool`` : 
		"""
		pass

	@staticmethod
	def IsFileInUse(filePath: str) -> bool:
		"""No Description

		Args
		--------
			filePath (``str``) :  filePath

		Returns
		--------
			``bool`` : 
		"""
		pass

	@staticmethod
	@property
	def LogSeparatorSize() -> int:
		"""No Description

		Returns
		--------
			``Util`` : 
		"""
		pass

	@staticmethod
	@property
	def LogSeparatorEquals() -> str:
		"""No Description

		Returns
		--------
			``Util`` : 
		"""
		pass

	@staticmethod
	@property
	def LogSeparatorDots() -> str:
		"""No Description

		Returns
		--------
			``Util`` : 
		"""
		pass

	@staticmethod
	@property
	def LogSeparatorXs() -> str:
		"""No Description

		Returns
		--------
			``Util`` : 
		"""
		pass

	@staticmethod
	@property
	def LogSeparatorUnderscores() -> str:
		"""No Description

		Returns
		--------
			``Util`` : 
		"""
		pass

	@staticmethod
	@property
	def LogSeparatorDashes() -> str:
		"""No Description

		Returns
		--------
			``Util`` : 
		"""
		pass

	@staticmethod
	@property
	def LogSeparatorPluses() -> str:
		"""No Description

		Returns
		--------
			``Util`` : 
		"""
		pass

	@staticmethod
	@property
	def LogSeparatorUpperBar() -> str:
		"""No Description

		Returns
		--------
			``Util`` : 
		"""
		pass

