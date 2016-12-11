#include <iostream>

//int sum(const int * data, int data_length)
//{
//	int result = 0;
//
//	_asm {
//		mov ecx, data; //Mov from data to eax
//		mov ebx, data_length;
//
//		xor edx, edx; //Reset to 0 as if you XOR and number with itself then it will return 0
//		xor eax, eax;
//	
//	l:
//		cmp edx, ebx; 
//		jge end; //jge jump to flag if previous CMP command is greater than or equal to
//		add eax, [ecx + edx * 4]; 
//		inc edx; //increment edx
//		jmp l; 
//	end:
//		mov result, eax;
//	}
//
//	return result;
//}

//int factorial(int order)
//{
//	//if (order == 1)
//	//	return 1;
//
//	//return order * factorial(order - 1);
//
//	int result = 0;
//
//	_asm {
//		cmp order, 1; //Compare order to 1
//		je one; //If equal, jump to one label
//		mov eax, order;
//		sub eax, 1; //Sub
//		
//		push eax; //push EAX to stack 
//		call factorial; //Returns to EAX
//		//pop ebx; //Pop last on stack to EBX which is the result of the push EAX command
//
//		imul eax, order; //order * result of function
//		mov result, eax;
//		jmp end;
//	one:
//		mov result, 1;
//	end:
//	}
//
//	return result;
//}

_declspec(naked) int factorial(int order)
{
	_asm {
		cmp [esp + 4], 1; //Compare argument to 1
		jne not_one; //If equal, jump to one label
		mov eax, 1;
		jmp end;

	not_one:
		mov eax, [esp + 4];
		sub eax, 1; //Sub

		push eax; //push EAX to stack 
		call factorial; //Returns to EAX
		add esp, 4; //Get rid of previous added value to esp (stack pointer) that was added in the last call

		imul eax, [esp + 4]; //order * result of function
	end:
		ret;
	}
}

_declspec(naked) int fibonnaci(int order)
{
	//int result = 0;

	//if (order <= 1) return 1;

	//result += fibonnaci(order - 1);
	//result += fibonnaci(order - 2);

	//return result;
	
	_asm{
		push ebp; //Save previous stackbase-pointer register
		mov ebp, esp; //
		
		push ebx; //Save previous ebx value
		move ebx, [ebp + 12];
		cmp ebx, 1;
		jle one;

		dec ebx;
		push ebx;
		call fibonnaci;
		mov[esp], eax;
		dec ebx;
		mov[esp], ebx;
		call fibonnaci;
		add eax, [edp];
		add esp, 4;
		pop ebx;
		add esp, 4;
		pop ebp;
		ret;

	one:
		pop ebx;


		mov eax, 1;
		ret;
	}
}

int main()
{
	return fibonnaci(5);
}