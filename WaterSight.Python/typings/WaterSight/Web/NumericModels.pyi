from enum import Enum

class WaterDomainElementTypeId(Enum):
	Pipe = 0
	Node = 1
	Pump = 2
	Tank = 3
	FCV = 4
	GPV = 5
	PBV = 6
	TCV = 7
	PRV = 8
	PSV = 9
	Reservoir = 10
	SCADA = 11
	Lateral = 12
	Customer = 13
	PumpStation = 14
	IsolationValve = 15

class SewerDomainElementTypeId(Enum):
	PondOutletStructure = 30
	CrossSectionNode = 31
	CatchBasin = 32
	Manhole = 33
	JunctionChamber = 34
	Pump = 35
	Outfall = 36
	WetWell = 37
	PressureJunction = 38
	AirValve = 39
	Headwall = 40
	PropertyConnection = 41
	Gutter = 42
	Conduit = 43
	PressurePipe = 44
	Channel = 45
	Catchment = 46
	Pond = 47
	LID = 48

