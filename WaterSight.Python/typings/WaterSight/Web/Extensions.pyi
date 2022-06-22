from typing import Iterator, List

class Extensions:

	def __init__(self) -> None:
		"""Creating a new Instance of this class is not allowed


		Raises
		--------
			Exception: if this class is instantiated
		"""
		raise Exception("Creating a new Instance of this class is not allowed")
		pass

	@staticmethod
	def GroupAt(source: Iterator[T], itemsPerGroup: int) -> Iterator[IEnumerable]:
		"""No Description

		Args
		--------
			source (``Iterator[T]``) :  source
			itemsPerGroup (``int``) :  itemsPerGroup

		Returns
		--------
			``Iterator[IEnumerable]`` : 
		"""
		pass

class DataFrameExt:

	def __init__(self) -> None:
		"""Creating a new Instance of this class is not allowed


		Raises
		--------
			Exception: if this class is instantiated
		"""
		raise Exception("Creating a new Instance of this class is not allowed")
		pass

	@staticmethod
	def IsEmpty(df: DataFrame) -> bool:
		"""No Description

		Args
		--------
			df (``DataFrame``) :  df

		Returns
		--------
			``bool`` : 
		"""
		pass

	@staticmethod
	def Shape(df: DataFrame) -> Tuple:
		"""No Description

		Args
		--------
			df (``DataFrame``) :  df

		Returns
		--------
			``Tuple`` : 
		"""
		pass

	@staticmethod
	def SplitByRows(df: DataFrame, capacity: int) -> List[DataFrame]:
		"""No Description

		Args
		--------
			df (``DataFrame``) :  df
			capacity (``int``) :  capacity

		Returns
		--------
			``List[DataFrame]`` : 
		"""
		pass

