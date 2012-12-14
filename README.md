GamesEngines1Assignment
=======================

Games Engines 1 Assignment -- 3D Visualisation of a brainfuck interpreter. 

Installation Instructions 
=======================

You can get the program in the following ways:

 * **GitHub**: https://github.com/eoinrogers/GamesEngines1Assignment.git
 * **WebCourses**: Program uploaded there
 * **The laptop you lent to me**: In the Documents\Games folder (please delete this after use; it contains identifying information, i.e. a student number)

If you are using the GitHub or WebCourses version, note that the 3 brainfuck programs in the Examples folder will have to be moved to the same directory as the executable to run the program: this is usually EoinRogersC09525386-GamesAssignment\Steering\Steering\bin\x86\Debug. 

Usage Instructions
=================

When you first run the program you will see a menu of all the brainfuck programs in the same directory as the program itself. Click on one to run it. You will see a 3D representation of the program running. Each row of blocks represents a single byte: one block represents one bit. Red means the bit is set to zero, green to one. 

The vehicle that moves down the side carries a read/write head on the top. You will see the head (called the wand in the source code) move to manipulate the values of bytes. 

Inputs to the program should be stored in a file called input.txt: this will essentially be stdin for the program. Outputs will be displayed on the screen, and written to a file called output.txt. 


