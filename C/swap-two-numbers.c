#include <stdio.h>

int main() 
{
	int a,b,c;

	printf("Enter a\n");
	scanf("%d",&a);
	printf("Enter b\n");
	scanf("%d",&b);

	c = a;
	a = b;
	b = c;

	printf("a = %d, b = %d", a, b);
	
}