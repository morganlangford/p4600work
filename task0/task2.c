/* Notes for Sept 12
Today we're gonna look at functions and pointers 

We looked at hexidecimal 

*/

#include <stdlib.h>	
#include <stdio.h>

/* POINTERS
int main(){

	
	int x;
	int y;
	int* p; // pointer to integer - instead of storing a number it stores an address

	x = 5;
	y = 7;
	p = &x; // make p point to the address of x (assign memory location)

	printf("x=%d\ny=%d\np=%p",x,y,p);

	printf("\n*p=%d",*p); // when the * is in front, it means what it's pointed TO
	// *p means access the pointee

	*p += 2; // this changes *p AND x!!
			// but it may crash computer -segmentation fault- so you have to be really careful about it
			// bUt you can change what it points to
	printf("\nx=%d\ny=%d\n*p=%d",x,y,*p);

	*/ 
	/* But why would we want to do this?
	Arrays and functions!! */


	/* POINTERS AND ARRAYS
	int array [] = {12,14,15,18};

	printf("%d",array[2]);

	printf("\nFor loop:");
	for (int i = 0; i<4; i++){
		printf("\n%d", array[i]);
	}

	// Or you could do this using pointers

	printf("\nPointers:");

	// This is considerably FASTER than for loop
	int* cursor = array;
	for (int j = 0; j<4; j++){
		printf("\n%d",*cursor);
		cursor++;
	}
	*/
//} 
// end of main



// Put your function before the main 

int divide(int a,int b, int* d){
	int c = a/b;
	*d = a%b; // modulus gives remainder

	return c; // stops executing this function, loads c into memory
}


void divideList(int* input, int divisor, int* output, int len){
	for (int i = 0; i<len; i++){
		output[i] = input[i]/divisor;
	}
	// void means return nothing
	// you could return a value like 1 or 0 to say hey it worked or it failed
}

int main(){
	int x = 5;
	int y = 3;
	int z;
	int rem;
	z = divide(x,y,&rem);

	printf("%d remainder %d",z,rem);

	int a[] = {1,2,3,5};
	int b[4];

	divideList(a,2,b,4);
}