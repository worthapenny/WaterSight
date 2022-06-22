from enum import Enum

class AlertOriginOriginType(Enum):
	Sensor = 0
	Zone = 1

class AlertType(Enum):
	Pattern = 1
	Absolute = 2
	NoData = 3
	FlatReading = 4

