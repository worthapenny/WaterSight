from typing import overload, Dict, TypeVar
from enum import Enum

T = TypeVar("T")

class Env(Enum):
	Prod = 0
	Qa = 1
	Dev = 2

class IntegrationType(Enum):
	Raw = 0
	Fifteen = 1
	Hourly = 2
	FifteenClean = 5
	HourlyClean = 6

class CRUDBase:

	def __init__(self) -> None:
		"""Creating a new Instance of this class is not allowed


		Raises
		--------
			Exception: if this class is instantiated
		"""
		raise Exception("Creating a new Instance of this class is not allowed")
		pass

	@staticmethod
	@overload
	def AddAsync(ws: WS, t: T, url: str, typeName: str) -> Task:
		"""No Description

		Args
		--------
			ws (``WS``) :  ws
			t (``T``) :  t
			url (``str``) :  url
			typeName (``str``) :  typeName

		Returns
		--------
			``Task`` : 
		"""
		pass

	@staticmethod
	@overload
	def AddAsync(ws: WS, url: str, typeName: str) -> Task:
		"""No Description

		Args
		--------
			ws (``WS``) :  ws
			url (``str``) :  url
			typeName (``str``) :  typeName

		Returns
		--------
			``Task`` : 
		"""
		pass

	@staticmethod
	def GetAsync(ws: WS, url: str, id: Nullable[int], typeName: str) -> Task:
		"""No Description

		Args
		--------
			ws (``WS``) :  ws
			url (``str``) :  url
			id (``Nullable``) :  id
			typeName (``str``) :  typeName

		Returns
		--------
			``Task`` : 
		"""
		pass

	@staticmethod
	def GetManyAsync(ws: WS, url: str, typeName: str) -> Task:
		"""No Description

		Args
		--------
			ws (``WS``) :  ws
			url (``str``) :  url
			typeName (``str``) :  typeName

		Returns
		--------
			``Task`` : 
		"""
		pass

	@staticmethod
	def UpdateAsync(ws: WS, id: Nullable[int], t: T, url: str, typeName: str, usePostMethod: bool = False) -> Task:
		"""No Description

		Args
		--------
			ws (``WS``) :  ws
			id (``Nullable``) :  id
			t (``T``) :  t
			url (``str``) :  url
			typeName (``str``) :  typeName
			usePostMethod (``bool``) :  usePostMethod

		Returns
		--------
			``Task`` : 
		"""
		pass

	@staticmethod
	def DeleteAsync(ws: WS, id: Nullable[int], url: str, typeName: str, supportsLRO: bool = False) -> Task:
		"""No Description

		Args
		--------
			ws (``WS``) :  ws
			id (``Nullable``) :  id
			url (``str``) :  url
			typeName (``str``) :  typeName
			supportsLRO (``bool``) :  supportsLRO

		Returns
		--------
			``Task`` : 
		"""
		pass

	@staticmethod
	def DeleteManyAsync(ws: WS, url: str, typeName: str, supportsLRO: bool = False) -> Task:
		"""No Description

		Args
		--------
			ws (``WS``) :  ws
			url (``str``) :  url
			typeName (``str``) :  typeName
			supportsLRO (``bool``) :  supportsLRO

		Returns
		--------
			``Task`` : 
		"""
		pass

	@staticmethod
	def PostAsync(ws: WS, url: str, content: HttpContent, typeName: str, supportsLRO: bool = False, additionalInfo: str = ) -> Task:
		"""No Description

		Args
		--------
			ws (``WS``) :  ws
			url (``str``) :  url
			content (``HttpContent``) :  content
			typeName (``str``) :  typeName
			supportsLRO (``bool``) :  supportsLRO
			additionalInfo (``str``) :  additionalInfo

		Returns
		--------
			``Task`` : 
		"""
		pass

	@staticmethod
	def PostFile(ws: WS, url: str, fileInfo: FileInfo, fileTypeName: str = Excel) -> Task:
		"""No Description

		Args
		--------
			ws (``WS``) :  ws
			url (``str``) :  url
			fileInfo (``FileInfo``) :  fileInfo
			fileTypeName (``str``) :  fileTypeName

		Returns
		--------
			``Task`` : 
		"""
		pass

	@staticmethod
	def PutAsync(ws: WS, url: str, content: HttpContent, typeName: str, supportsLRO: bool = False, additionalInfo: str = ) -> Task:
		"""No Description

		Args
		--------
			ws (``WS``) :  ws
			url (``str``) :  url
			content (``HttpContent``) :  content
			typeName (``str``) :  typeName
			supportsLRO (``bool``) :  supportsLRO
			additionalInfo (``str``) :  additionalInfo

		Returns
		--------
			``Task`` : 
		"""
		pass

class Payload:

	def __init__(self) -> None:
		"""Creating a new Instance of this class is not allowed


		Raises
		--------
			Exception: if this class is instantiated
		"""
		raise Exception("Creating a new Instance of this class is not allowed")
		pass

	@staticmethod
	@property
	def P0F015M() -> Dict[str,str][str,object]:
		"""No Description

		Returns
		--------
			``Payload`` : 
		"""
		pass

class Request:

	def __init__(self) -> None:
		"""Creating a new Instance of this class is not allowed


		Raises
		--------
			Exception: if this class is instantiated
		"""
		raise Exception("Creating a new Instance of this class is not allowed")
		pass

	@staticmethod
	def Get(url: str) -> Task:
		"""No Description

		Args
		--------
			url (``str``) :  url

		Returns
		--------
			``Task`` : 
		"""
		pass

	@staticmethod
	def Put(url: str, content: HttpContent) -> Task:
		"""No Description

		Args
		--------
			url (``str``) :  url
			content (``HttpContent``) :  content

		Returns
		--------
			``Task`` : 
		"""
		pass

	@staticmethod
	def Post(url: str, content: HttpContent, timeout: Nullable = None) -> Task:
		"""No Description

		Args
		--------
			url (``str``) :  url
			content (``HttpContent``) :  content
			timeout (``Nullable``) :  timeout

		Returns
		--------
			``Task`` : 
		"""
		pass

	@staticmethod
	def PostJsonString(url: str, jsonString: str) -> Task:
		"""No Description

		Args
		--------
			url (``str``) :  url
			jsonString (``str``) :  jsonString

		Returns
		--------
			``Task`` : 
		"""
		pass

	@staticmethod
	def PutJsonString(url: str, jsonString: str) -> Task:
		"""No Description

		Args
		--------
			url (``str``) :  url
			jsonString (``str``) :  jsonString

		Returns
		--------
			``Task`` : 
		"""
		pass

	@staticmethod
	def PostFile(url: str, fileInfo: FileInfo, timeout: Nullable = None) -> Task:
		"""No Description

		Args
		--------
			url (``str``) :  url
			fileInfo (``FileInfo``) :  fileInfo
			timeout (``Nullable``) :  timeout

		Returns
		--------
			``Task`` : 
		"""
		pass

	@staticmethod
	def Delete(url: str) -> Task:
		"""No Description

		Args
		--------
			url (``str``) :  url

		Returns
		--------
			``Task`` : 
		"""
		pass

	@staticmethod
	def GetJsonAsync(response: HttpResponseMessage) -> Task:
		"""No Description

		Args
		--------
			response (``HttpResponseMessage``) :  response

		Returns
		--------
			``Task`` : 
		"""
		pass

	@staticmethod
	def WaitForLRO(res: HttpResponseMessage, checkIntervalSeconds: int = 5) -> Task:
		"""No Description

		Args
		--------
			res (``HttpResponseMessage``) :  res
			checkIntervalSeconds (``int``) :  checkIntervalSeconds

		Returns
		--------
			``Task`` : 
		"""
		pass

	@staticmethod
	@property
	def Options() -> Options:
		"""No Description

		Returns
		--------
			``Request`` : 
		"""
		pass

	@staticmethod
	@property
	def BearerToken() -> str:
		"""No Description

		Returns
		--------
			``Request`` : 
		"""
		pass

class WSItem:

	def __init__(self, ws: WS) -> None:
		"""Creating a new Instance of this class is not allowed


		Raises
		--------
			Exception: if this class is instantiated
		"""
		raise Exception("Creating a new Instance of this class is not allowed")
		pass

	@property
	def WS(self) -> WS:
		"""No Description

		Returns
		--------
			``WSItem`` : 
		"""
		pass

	@property
	def EndPoints(self) -> EndPoints:
		"""No Description

		Returns
		--------
			``WSItem`` : 
		"""
		pass

	@property
	def Logger(self) -> ILogger:
		"""No Description

		Returns
		--------
			``WSItem`` : 
		"""
		pass

