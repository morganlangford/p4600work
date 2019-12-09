// Pointers store the memory address

/*
int b;
int *a; 	// a is a pointer
int *a = &b;	// &b is the memory location of the pointer *a
// so it holds a memory address instead of data
*/

#include <stdio.h>
#include <string.h>

int main()
{
	int a = 55;
	int *b;
	b = &a;

	printf("Value of a = %d\n",a);		// value
	printf("Address of a = %d\n",&a);	// address
	printf("Value of b = %d\n",b);		// same address

	return 0;
}