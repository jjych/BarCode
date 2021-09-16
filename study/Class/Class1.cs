using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace study.Class
{
    class PACEBarcode_D
    {
        // 1D Barcode
        public const int PACE_BARCODETYPE_CODE39 = 0x0001;	// 1D BARCODE CODE39
        public const int PACE_BARCODETYPE_CODE93 = 0x0002;	// 1D BARCODE CODE93
        public const int PACE_BARCODETYPE_CODE128 = 0x0004;	// 1D BARCODE CODE128
        public const int PACE_BARCODETYPE_CODABAR = 0x0008;	// 1D BARCODE CODABAR
        public const int PACE_BARCODETYPE_I2OF5 = 0x0010;	// 1D BARCODE I2OF5
        public const int PACE_BARCODETYPE_EAN = 0x0020;	// 1D BARCODE EAN
        public const int PACE_BARCODETYPE_UPC = 0x0040;		// 1D BARCODE UPC
        // 2D Barcode
        public const int PACE_BARCODETYPE_PDF417 = 0x0100;		// 2D BARCODE PDF417
        public const int PACE_BARCODETYPE_DATAMATRIX = 0x0200;		// 2D BARCODE DATAMATRIX
        public const int PACE_BARCODETYPE_QRCODE = 0x0400;		// 2D BARCODE QRCODE
        public const int PACE_BARCODETYPE_CODEONE = 0x0800;	// 2D BARCODE CODEONE
        public const int PACE_BARCODETYPE_MAXICODE = 0x1000;	// 2D BARCODE MAXICODE
        // 2D Barcode Ex
        public const int PACE_BARCODETYPE_D2RMATRIX = 0x8000;	// 2D BARCODE D2RMATRIX

        [DllImport("PACEBarcode_D.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        //public static extern int PACEBarcode_D_Region(int type, int upx, int upy, int dnx, int dny, string filename, StringBuilder position, StringBuilder result);
        public static extern int PACEBarcode_D_Region(int type, int upx, int upy, int dnx, int dny, string filename, StringBuilder position, byte[] result);

        [DllImport("PACEBarcode_D.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]

        public static extern int PACEBarcode_D_Region(int type, int upx, int upy, int dnx, int dny, string filename, StringBuilder position, StringBuilder result);



        [DllImport("PACEBarcode_D.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int PACEBarcode_D_Auto(int type, string filename, StringBuilder position, StringBuilder result);


        [DllImport("PACEBarcode_D.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int PACEBarcode_D_Auto(int type, string filename, StringBuilder position, byte[] result);

    }
}

///////******************************************************************************

//////    PACEBarcode_D.H : BARCODE

//////    Copyright by PACE System Co., Ltd 2004-2014
//////    All rights reserved.

//////    defined for 32-bit environment
//////    may need porting if working for 64-bit environment

//////******************************************************************************/

//////// 바코드 인식 모듈 
//////// 모듈명 : PACEBarcode_D.DLL
//////// 바코드를 찾고 인식을 수행한다. 
///////*
//////    - 인식 대상 
//////        1d : code39 / code93 / code128 / codabar / i2of5 / ean / upc 
//////        2d : pdf417 / datamatrix / codeone / maxicode / qrcode 
//////        2dex : datamatrix2
//////    - 바코드 자동 검출 인식 
//////    - 바코드 수동 검출 인식 
//////*/

//////#ifndef _PACEBarcode_D
//////#define _PACEBarcode_D

//////// 1D Barcode
//////#define PACE_BARCODETYPE_CODE39			0x0001		// 1D BARCODE CODE39
//////#define PACE_BARCODETYPE_CODE93			0x0002		// 1D BARCODE CODE93
//////#define PACE_BARCODETYPE_CODE128			0x0004		// 1D BARCODE CODE128
//////#define PACE_BARCODETYPE_CODABAR			0x0008		// 1D BARCODE CODABAR
//////#define PACE_BARCODETYPE_I2OF5			0x0010		// 1D BARCODE I2OF5
//////#define PACE_BARCODETYPE_EAN				0x0020		// 1D BARCODE EAN
//////#define PACE_BARCODETYPE_UPC				0x0040		// 1D BARCODE UPC
//////// 2D Barcode
//////#define PACE_BARCODETYPE_PDF417			0x0100		// 2D BARCODE PDF417
//////#define PACE_BARCODETYPE_DATAMATRIX		0x0200		// 2D BARCODE DATAMATRIX
//////#define PACE_BARCODETYPE_QRCODE			0x0400		// 2D BARCODE QRCODE
//////#define PACE_BARCODETYPE_CODEONE			0x0800		// 2D BARCODE CODEONE
//////#define PACE_BARCODETYPE_MAXICODE		0x1000		// 2D BARCODE MAXICODE
//////// 2D Barcode Ex
//////#define PACE_BARCODETYPE_D2RMATRIX		0x8000		// 2D BARCODE D2RMATRIX


//////#if defined(__cplusplus)
//////extern "C" {
//////#endif

////////unsigned char *APIENTRY PACEBarcode_D_Version(void);

//////// 바코드가 몇개 존재하는가를 Check 
//////// return : 0<:인식된 바코드 수, 0 인식실패, -1 검색실패, -2 변수오류, -3 함수 로드실패, -4 파일로드실패 
//////int APIENTRY PACEBarcode_D_Auto_Check(int type,			// 바코드 타입 
//////                                                        // PACE_BARCODETYPE_* 참조 OR 연산 가능
//////                            unsigned char *filename);	// 이미지 파일명

//////// 바코드를 자동검출 인식한다.	==> 중요 : 압축모드를 쓰면 Unzip 함수를 쓰시오.
//////// normal api function
//////// return : 0<:인식된 바코드 수, 0 인식실패, -1 검색실패, -2 변수오류, -3 함수 로드실패, -4 파일로드실패 
//////int APIENTRY PACEBarcode_D_Auto(int type,				// 바코드 타입 
//////                                                        // PACE_BARCODETYPE_* 참조 OR 연산 가능
//////                            unsigned char *filename,	// 이미지 파일명
//////                            unsigned char *position,	// position string 
//////                                                        // left1|top1|right1|bottom1|sympos1|symnum1|fileid1|...형식임
//////                            unsigned char *result);		// Result String [20000] : 구분자 "ⓟⓐⓒⓔ"

//////// data unzip api function
//////// return : 0<:인식된 바코드 수, 0 인식실패, -1 검색실패, -2 변수오류, -3 함수 로드실패, -4 파일로드실패 
//////int APIENTRY PACEBarcode_D_Auto_Unzip(int type,			// 바코드 타입 
//////                                                        // PACE_BARCODETYPE_* 참조 OR 연산 가능
//////                            unsigned char *filename,	// 이미지 파일명
//////                            unsigned char *position,	// position string 
//////                                                        // left1|top1|right1|bottom1|sympos1|symnum1|fileid1|...형식임
//////                            unsigned char *result);		// Result String [20000] : 구분자 "ⓟⓐⓒⓔ"

///////*	datamatrix의 position 변수는 바코드 위치를 위한 left, top, right, bottom 과 
//////    symbol position (1~16) : 하나의 데이타를 공유하는 여러개 바코드중 몇번째 인가를 나타내는 값
//////    symbol number (1~16) : 하나의 데이타를 여러개의 바코드에 나누어 담게 되었을때 총 바코드의 수
//////    file id (1~64516) : 여러개의 바코드에 담게된 데이타 마다의 번호표기
//////*/

//////// 바코드가 몇개 존재하는가를 Check 
//////// return : 0<:인식된 바코드 수, 0 인식실패, -1 검색실패, -2 변수오류, -3 함수 로드실패, -4 파일로드실패 
//////int APIENTRY PACEBarcode_D_Region_Check(int type,		// 바코드 타입 
//////                                                        // PACE_BARCODETYPE_* 참조 OR 연산 가능
//////                            int upx,					// 
//////                            int upy,					// 
//////                            int dnx,					// 
//////                            int dny,					// 
//////                            unsigned char *filename);	// 이미지 파일명

//////// 바코드를 자동검출 인식한다.	==> 중요 : 압축모드를 쓰면 Unzip 함수를 쓰시오.
//////// normal api function
//////// return : 0<:인식된 바코드 수, 0 인식실패, -1 검색실패, -2 변수오류, -3 함수 로드실패, -4 파일로드실패 
//////int APIENTRY PACEBarcode_D_Region(int type,				// 바코드 타입 
//////                                                        // PACE_BARCODETYPE_* 참조 OR 연산 가능
//////                            int upx,					// 
//////                            int upy,					// 
//////                            int dnx,					// 
//////                            int dny,					// 
//////                            unsigned char *filename,	// 이미지 파일명
//////                            unsigned char *position,	// position string 
//////                                                        // left1|top1|right1|bottom1|sympos1|symnum1|fileid1|...형식임
//////                            unsigned char *result);		// Result String [20000] : 구분자 "ⓟⓐⓒⓔ"

//////// data unzip api function
//////// return : 0<:인식된 바코드 수, 0 인식실패, -1 검색실패, -2 변수오류, -3 함수 로드실패, -4 파일로드실패 
//////int APIENTRY PACEBarcode_D_Region_Unzip(int type,		// 바코드 타입 
//////                                                        // PACE_BARCODETYPE_* 참조 OR 연산 가능
//////                            int upx,					// 
//////                            int upy,					// 
//////                            int dnx,					// 
//////                            int dny,					// 
//////                            unsigned char *filename,	// 이미지 파일명
//////                            unsigned char *position,	// position string 
//////                                                        // left1|top1|right1|bottom1|sympos1|symnum1|fileid1|...형식임
//////                            unsigned char *result);		// Result String [20000] : 구분자 "ⓟⓐⓒⓔ"

///////*	datamatrix의 position 변수는 바코드 위치를 위한 left, top, right, bottom 과 
//////    symbol position (1~16) : 하나의 데이타를 공유하는 여러개 바코드중 몇번째 인가를 나타내는 값
//////    symbol number (1~16) : 하나의 데이타를 여러개의 바코드에 나누어 담게 되었을때 총 바코드의 수
//////    file id (1~64516) : 여러개의 바코드에 담게된 데이타 마다의 번호표기
//////*/

//////// 바코드가 몇개 존재하는가를 Check 
//////// return : 0<:인식된 바코드 수, 0 인식실패, -1 검색실패, -2 변수오류, -3 함수 로드실패, -4 파일로드실패 
//////int APIENTRY PACEBarcode_D_Memory_Check(int type,		// 바코드 타입 
//////                                                        // PACE_BARCODETYPE_* 참조 OR 연산 가능
//////                            int bpp,					//
//////                            int xsize,					// 
//////                            int ysize,					// 
//////                            unsigned char **image);		// 이미지 버퍼

//////// 바코드를 자동검출 인식한다.	==> 중요 : 압축모드를 쓰면 Unzip 함수를 쓰시오.
//////// normal api function
//////// return : 0<:인식된 바코드 수, 0 인식실패, -1 검색실패, -2 변수오류, -3 함수 로드실패, -4 파일로드실패 
//////int APIENTRY PACEBarcode_D_Memory(int type,				// 바코드 타입 
//////                                                        // PACE_BARCODETYPE_* 참조 OR 연산 가능
//////                            int bpp,					//
//////                            int xsize,					// 
//////                            int ysize,					// 
//////                            unsigned char **image,		// 이미지 버퍼
//////                            unsigned char *position,	// position string 
//////                                                        // left1|top1|right1|bottom1|sympos1|symnum1|fileid1|...형식임
//////                            unsigned char *result);		// Result String [20000] : 구분자 "ⓟⓐⓒⓔ"

//////// data unzip api function
//////// return : 0<:인식된 바코드 수, 0 인식실패, -1 검색실패, -2 변수오류, -3 함수 로드실패, -4 파일로드실패 
//////int APIENTRY PACEBarcode_D_Memory_Unzip(int type,		// 바코드 타입 
//////                                                        // PACE_BARCODETYPE_* 참조 OR 연산 가능
//////                            int bpp,					//
//////                            int xsize,					// 
//////                            int ysize,					// 
//////                            unsigned char **image,		// 이미지 버퍼
//////                            unsigned char *position,	// position string 
//////                                                        // left1|top1|right1|bottom1|sympos1|symnum1|fileid1|...형식임
//////                            unsigned char *result);		// Result String [20000] : 구분자 "ⓟⓐⓒⓔ"

///////*	datamatrix의 position 변수는 바코드 위치를 위한 left, top, right, bottom 과 
//////    symbol position (1~16) : 하나의 데이타를 공유하는 여러개 바코드중 몇번째 인가를 나타내는 값
//////    symbol number (1~16) : 하나의 데이타를 여러개의 바코드에 나누어 담게 되었을때 총 바코드의 수
//////    file id (1~64516) : 여러개의 바코드에 담게된 데이타 마다의 번호표기
//////*/

//////#if defined(__cplusplus)
//////}
//////#endif

//////#endif /*	_PACEBarcode_D	*/
