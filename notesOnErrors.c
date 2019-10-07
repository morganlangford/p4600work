/*  The four levels of error:
	- Compiler error:
	error vs warning: if an error happens, it won't compile your code; 
	doesn't produce a .o file. With a warning it still does - but 
	it's probably wrong. Will compile .o file, not .exe though
	These are compiler errors. Often things like misspelling/syntax 
	(missing ;, assigning things of the wrong type) 
	They're tedious, usually easy to find, easy to fix
	
	- Linker error:
	Basically if you compile your main, all functions and headers
	then when your computer turns that all into one big piece
	of machine code, it can't find what to do with one of the functions
	Could even be something like math.h
	Probably because you made a change like findMean(float x) to 
	findMean(float x,float y) but you haven't updated that everywhere

	- Runtime error:
	Math error - divide by zero DIV0, log of -'ve, sqrt(-'ve), NAN
	Find by testing all kinds of inputs and data
	Segmentation fault:
	When you try to access memory that you're not supposed to access
	Your program will definitely stop

	- Logic error:
	It's running, working, giving you answers - but the answers are wrong
	Difficult to catch because you don't know if the answer is right or wrong
	unless you check it 
	Check it by giving it values that you know should produce a certain value
	or that you know should be wrong
	You can write a separate program that makes values to test

	*/