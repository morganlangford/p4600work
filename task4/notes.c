// Week 4 notes
#include <stdio.h>
#include <stdlib.h>
#include <math.h>

// We're trying to write universal code that can be used in multiple programs 

#define MAX_N_DATA 128  
/* 	Helpful if you're going to use the same number multiple times or you'll need to 
		change it depending on what you're using the code for
	It's a good idea to use as few numbers in your actually code as you can
	this is called a macro
	to use it, the name must be in all caps
	no semi-colon! */


// This is a global variable (outside the main):
int max_n_data = 128;


/*  We can now have a program with only a main that calls functions from other programs 
	that are in a totally separate file
	But how do we access them? Header files 
	so we'll have a program main.c - have to include statistics.h, have a program 
	called statistics.c - includes stats.h, then a statistics.h file that just has 
	instructions in it */

#include "statistics.h"    // it's in "" because it's in the same directory

//the statistics.h file could look like:

#ifndef __STATISTICS_H 
#define __STATISTICS_H

#include <std.io>

#define MAX_N_DATA 128;

double stdevf(double* data, int n, double* meanout);
int load_data_columns(FILE* fp, double* xdata, double* ydata, int max_n);

#endif


/* When you're compiling, you'll say 
	gcc -c main.c statistics.c  	//where -c means something I forget but do it 
	ls
	gcc main.o statistics.o
	*/

// ONLY ONE FILE CAN HAVE A main