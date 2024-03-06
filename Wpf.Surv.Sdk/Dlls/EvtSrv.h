#pragma once

#ifdef EVTSRV_EXPORTS
#define EVTSRV_API __declspec(dllexport)
#else
#define EVTSRV_API __declspec(dllimport)
#endif


#ifdef __cplusplus
extern "C" {
#endif
	typedef void* EsHandle;
	typedef struct _LprInfo {
		int		nCamChannel;
		char	*pszCamAaddress;
		char	*pszUtc;
		char	*pszLprNum;
		char	*pJpeg;
		int		nJpegSize;
		int		nLineNo;
		bool	bDrawLpr;
		bool	bDrawJpeg;
	} LprInfo;

	typedef struct _FaceInfo {
		int		nCamChannel;
		char	*pszCamAaddress;
		char	*pszUtc;
		char	*pszName;
		int		nAge;		// 0:Unknown, 1~
		int		nGender;	// 0:Unknown, 1: Male, 2: Female
		char	*pszAdditional;
		char	*pJpeg;
		int		nJpegSize;
		bool	bDrawJpeg;
	} FaceInfo;

	typedef struct _EventInfo {
		int		nCamChannel;
		char	*pszCamAaddress;
		char	*pszEventName; //Motion/Alarm/EmergencyBell/Temp/Pir/Door
		bool	bOn;	
		int		nId;
	} EventInfo;

	typedef struct _CustomParam {
		char	*pszName;
		char	*pszValue;
	} CustomParam;

	typedef struct _CustomInfo {
		bool		bSearch;
		int			nChannelArrayLen;
		int			*pChannelArray;
		int			nParamArrayLen;
		CustomParam *pParamArray;
	} CustomInfo;

	EVTSRV_API EsHandle	EsCreateInstance	( char *szAddr, unsigned short nPort, char *szId, char *szPwd );
	EVTSRV_API void		EsDeleteInstance	( EsHandle hEs );

	EVTSRV_API bool		EsSendMessage		( EsHandle hEs, char *szMessage, int nMessageLen );
	EVTSRV_API bool		EsSendLpr			( EsHandle hEs, LprInfo *pLpr );
	EVTSRV_API bool		EsSendFace			( EsHandle hEs, FaceInfo *pLpr );
	EVTSRV_API bool		EsSendEvent			( EsHandle hEs, EventInfo *pEvt );
	EVTSRV_API bool		EsSendCustom		( EsHandle hEs, CustomInfo *pInfo );

	EVTSRV_API bool		EsSkipRecoder		( EsHandle hEs, bool bSet );

#ifdef __cplusplus
}
#endif