# ConnectionStringCat
This Visual Studio package provides an easy way to switch connection strings and other items in config files across the Visual Studio solution.
In most cases defining custom solution configurations with corresponding config file transformations (https://msdn.microsoft.com/en-us/library/dd465326(v=vs.110).aspx) or just changing configuration manually would be enough. However when your solution grows large you would probably have several volatile config files and a number of different environment options (multiple development and test databases, web-service addresses or logging settings), some of wich should be changed in sync with each other, others don't. In that case using config transforms would give you a combinatorial explosion of the number of solution combinations, and changing config files every time manually in a number of projects would be tiresome and onerous. 
ConnectionStringCat provides a much easier approach to handle such situations. You specify all your configuration options and rules how to apply them in a separate JSON file and ConnectionStringCat gives youa tool to switch configuration variants right in your Visual Studio with a few clicks.
#Demo
Here is a small demonstration of ConnectionStringCat in work
![ConnectionStringCat Demo](https://github.com/RegiSV2/ConnectionStringCat/blob/master/doc/demo.gif)

#Supported Versions
Supports Visual Studio 2012 Pro+
