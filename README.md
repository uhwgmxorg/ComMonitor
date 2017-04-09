# ComMonitor
Com Monitor is a small but complete utility for playing and testing with TCP connections and sending messages over socket connections. It is written in C# using WPF and the .Net 4.5 framework. For example it can be useful for PLC developers, to test connections and communication with the PLC or other devices.

![img](https://github.com/uhwgmxorg/ComMonitor/blob/master/Doc/1.png)

ComMonitor make it possible to send, receive, edit, rename, save and relode TCP-Messages. A connection is represented in a MDI-Window. If a MDI-Window is connected, an exclamation mark indicates the connection state:

![img](https://github.com/uhwgmxorg/ComMonitor/blob/master/Doc/2.png)

 New message can be add by pressing the *Add New Message* button. This will open a DialogBox with the cool WPFHexEditorControl from [abbaye](https://github.com/abbaye/WPFHexEditorControl):

![img](https://github.com/uhwgmxorg/ComMonitor/blob/master/Doc/3.png)

To create a new message, enter the size of the new message then press Ok

![img](https://github.com/uhwgmxorg/ComMonitor/blob/master/Doc/4.png)

Each MDI window has its own message list with a Focus Message. The Focus Message is the message of the top item of the tab control, it is sent when pressing the Focus Message Sent ToolBar button.:

![img](https://github.com/uhwgmxorg/ComMonitor/blob/master/Doc/7.png)

Message names can be changed by double clicking on the tab control item:

![img](https://github.com/uhwgmxorg/ComMonitor/blob/master/Doc/6.png)

With the Edit Message ToolBar command you can recall and edit the associated messages of an MDI window:

![img](https://github.com/uhwgmxorg/ComMonitor/blob/master/Doc/5.png)

Storage of messages and connection configurations is possible using ToolBars commands:

![img](https://github.com/uhwgmxorg/ComMonitor/blob/master/Doc/8.png)

Pleasant refer the Change Log and List Of Known Bugs for further information:

![img](https://github.com/uhwgmxorg/ComMonitor/blob/master/Doc/9.png)
