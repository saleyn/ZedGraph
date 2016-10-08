# ZedGraph #

This is a fork of https://github.com/discomurray/ZedGraph.

The original project is LGPL copyrighted 2004-2009 by John Champion.

In this fork we try to bring this project up to date to take
advandage of modern C# features as well as add functionality
to make it more usable for financial charting.

## Added features ##

* Auto-updating horizontal line levels in the GraphPane
* Crosshair drawing
* Chart scrolling with MouseWheel
* Chart zooming band with fill effect
* Controlling panning/zooming actions using chart's MouseWheelActions property
* Chart panning/zooming/by using modifier keys and mouse wheel on the XAxis or YAxis
* Mouse selectable LineItem
* Annotation shapes (PointObj)
* SplitterPane and ability to adjust pane sizes within MasterPane using SplitterPane
* Modify PointPair to implement IPointPair interface and convert all data lists to
  support IPointPair.
* Modify StockPt to implement IStockPt interface and convert StockPointList to
  support IStockPt.

Example:

![image](https://cloud.githubusercontent.com/assets/272543/18227320/ce021676-71ee-11e6-9e54-78b8bfe64e8d.png)

![image](https://cloud.githubusercontent.com/assets/272543/18622921/60c71ef4-7e04-11e6-91b1-7373bd4e0291.png)

## Maintainer ##

Serge Aleynikov
(https://github.com/saleyn/ZedGraph)

## LICENSE ##

LGPL v2.1

