// PlatDev_01.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

#include <iostream>

int add(unsigned short x, unsigned short y ,unsigned short z)
{
	int result;

	_asm //Keyword to mark code as use assembly
	{
		xor ebx, ebx; //XOR itself with 0, fastest way to move 0 into a registers
		mov bx, x;
		add bx, y; //Becomes 0 as it's 16 bit. Adding these becomes 1 in the 17th byte, overflows.
		add bx, z;
		mov result, ebx;
	}

	return result;
}

int abs(int x)
{
	int result;

	/*
		if(x < 0)
			return -x;

		return x;
	*/

	_asm
	{
		mov ebx, x;

		cmp ebx, 0;
		jg end

		neg ebx;

	end:
		mov result, ebx;
	}

	return result;
}

int loop(int x, int y)
{
	int result;
	int i = 0;
	/*
	for(int i = 0; i < x; ++i)
		result += y;

		add ebx, 1 === inc ebx
	*/

	_asm
	{

		/* An example of a do while loop
		xor eax, eax;

		mov ebx, x;
	forLoop:
		add eax, y;

		dec ebx;

		cmp ebx, 0;
		jg forLoop

		mov result, eax;
		*/

		mov ecx, x; //Move x into register
		xor ebx, ebx; //Clear eax to 0

	myLoop: //label
		add ebx, y; //add y to ebx
		inc i; //increment i
		cmp i, ecx; //compare i with ecx
		jl myLoop; //jump to myLoop label

		mov result, ebx;
	}

	return result;
}

int mathAbs(int x)
{
	if (((0b10000000) & x) != 0) //The 0b notation works in code!
		return ~x + 1; //add one as otherwise it's out of alignment.

	return x;
}

int Add(int x, int y)
{
	//half adder is a ^ and an & 
	int result = 0;
	int carry = 0;
	for (int i = 0; i < 8; ++i)
	{
		int a = x >> i & 0b00000001;//Mask the bit you want by shifting right then & masking away
		int b = y >> i & 0b00000001;

		int xab = a ^ b;
		int sum = carry ^ xab; //Final sum

		carry = (carry & xab) | (a & b);

		result = result | (sum << i);
	}

	return result;
}

int main()
{
    std::cout << add(1 << 15, 1 << 15, 1 << 15) << "\n";
	std::cout << abs(-5) << "\n";
	std::cout << loop(2, 30) << "\n";
	std::cout << mathAbs(12) << "\n";
	std::cout << mathAbs(-24) << "\n";
	std::cout << mathAbs(-41) << "\n";
	std::cout << "Add: " << Add(5, 10) << "\n";
	std::cout << "Add: " << Add(5, 11) << "\n";
	std::cout << "Add: " << Add(64, 90) << "\n";
	std::cout << "Add: " << Add(50, -10) << "\n";
	return 0;
}

