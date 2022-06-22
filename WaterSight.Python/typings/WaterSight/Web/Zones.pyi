from enum import Enum

class WaterLossMethod(Enum):
	BasedOnMNF = 0
	AlPercentOfInput = 1
	AlPercentOfRevenue = 2
	AlPercentOfLosses = 3

