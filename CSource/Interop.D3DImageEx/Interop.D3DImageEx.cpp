#include "stdafx.h"
#include "Interop.D3DImageEx.h"
#include <stdexcept>
#include <limits>

namespace System {
	namespace Windows {
		namespace Interop
		{

			// 클래스 생성자
			static D3DImageEx::D3DImageEx()
			{
				InitD3D9(GetDesktopWindow());
			}

			IntPtr D3DImageEx::CreateBackBuffer(D3DResourceTypeEx resourceType, IntPtr pResource)
			{
				/* Check if the user just wants to clear the D3DImage backbuffer */
				if (pResource == IntPtr::Zero)
				{
					if (_backBuffer != IntPtr::Zero)
					{
						IDirect3DSurface9* pSurface = static_cast<IDirect3DSurface9*>(_backBuffer.ToPointer());
						pSurface->Release();
						_backBuffer = IntPtr::Zero;
					}
					return IntPtr::Zero;
				}

				IUnknown* pUnk = static_cast<IUnknown*>(pResource.ToPointer());

				UINT width = 0;
				UINT height = 0;
				DXGI_FORMAT format = DXGI_FORMAT_UNKNOWN;
				IDirect3DSurface9* pSurface = nullptr;

				/* D3D version specific stuffs */
				// 리소스 타입에 따라 적절한 Direct3D 텍스처 인터페이스를 얻습니다.
				switch (resourceType)
				{
				case D3DResourceTypeEx::ID3D10Texture2D:
				{
					ID3D10Texture2D* pTexture2D = nullptr;
					// IUnknown에서 ID3D10Texture2D 인터페이스를 얻습니다.
					if (FAILED(pUnk->QueryInterface(__uuidof(ID3D10Texture2D), reinterpret_cast<void**>(&pTexture2D))))
					{
						throw gcnew ArgumentException("Invalid resource for ID3D10Texture2D", "pResource");
					}
					D3D10_TEXTURE2D_DESC textureDesc;
					pTexture2D->GetDesc(&textureDesc); // 텍스처 정보 얻기
					width = textureDesc.Width;
					height = textureDesc.Height;
					format = textureDesc.Format;
					pTexture2D->Release(); // 리소스 해제
					break;
				}
				case D3DResourceTypeEx::ID3D11Texture2D:
				{
					ID3D11Texture2D* pTexture2D = nullptr;
					// IUnknown에서 ID3D11Texture2D 인터페이스를 얻습니다.
					if (FAILED(pUnk->QueryInterface(__uuidof(ID3D11Texture2D), reinterpret_cast<void**>(&pTexture2D))))
					{
						throw gcnew ArgumentException("Invalid resource for ID3D11Texture2D", "pResource");
					}
					D3D11_TEXTURE2D_DESC textureDesc;
					pTexture2D->GetDesc(&textureDesc); // 텍스처 정보 얻기
					width = textureDesc.Width;
					height = textureDesc.Height;
					format = textureDesc.Format;
					pTexture2D->Release(); // 리소스 해제
					break;
				}
				default:
					throw gcnew ArgumentOutOfRangeException("resourceType");
				}

				/* The shared handle of the D3D resource */
				HANDLE hSharedHandle = NULL;

				/* Shared texture pulled through the 9Ex device */
				IDirect3DTexture9* pTexture = nullptr;


				/* Get the shared handle for the given resource */
				if (FAILED(GetSharedHandle(pUnk, &hSharedHandle)))
				{
					throw gcnew Exception("Could not aquire shared resource handle");
				}

				/* Get the shared surface.  In this case its really a texture =X */
				if (FAILED(GetSharedSurface(hSharedHandle, (void**)&pTexture, width, height, format)))
				{
					throw gcnew Exception("Could not create shared resource");
				}

				/* Get surface level 0, which we need for the D3DImage */
				if (FAILED(pTexture->GetSurfaceLevel(0, &pSurface)))
				{
					pTexture->Release();
					throw gcnew Exception("Could not get surface level");
				}

				// 기존 백버퍼가 있다면 해제합니다.
				if (_backBuffer != IntPtr::Zero)
				{
					IDirect3DSurface9* pOldSurface = static_cast<IDirect3DSurface9*>(_backBuffer.ToPointer());
					pOldSurface->Release();
				}

				// 백버퍼를 업데이트합니다.
				_backBuffer = IntPtr(pSurface);
				pTexture->Release(); // 텍스처 해제
				return _backBuffer;
			}

			IntPtr D3DImageEx::GetBackbuffer()
			{
				return _backBuffer;
			}

			D3DFORMAT D3DImageEx::ConvertDXGIToD3D9Format(DXGI_FORMAT format)
			{
				switch (format)
				{
				case DXGI_FORMAT_B8G8R8A8_UNORM:
					return D3DFMT_A8R8G8B8;
				case DXGI_FORMAT_B8G8R8A8_UNORM_SRGB:
					return D3DFMT_A8R8G8B8;
				case DXGI_FORMAT_B8G8R8X8_UNORM:
					return D3DFMT_X8R8G8B8;
				case DXGI_FORMAT_R8G8B8A8_UNORM:
					return D3DFMT_A8B8G8R8;
				case DXGI_FORMAT_R8G8B8A8_UNORM_SRGB:
					return D3DFMT_A8B8G8R8;
				default:
					return D3DFMT_UNKNOWN;
				};
			}

			HRESULT D3DImageEx::GetSharedSurface(HANDLE hSharedHandle, void** ppUnknown, UINT width, UINT height, DXGI_FORMAT format)
			{
				D3DFORMAT d3d9Format = ConvertDXGIToD3D9Format(format);
				if (d3d9Format == D3DFMT_UNKNOWN)
				{
					return E_INVALIDARG;
				}

				HRESULT hr = S_OK;
				IDirect3DTexture9** ppTexture = (IDirect3DTexture9**)ppUnknown;

				/* Create the texture locally, but provide the shared handle.
				 * This doesn't really create a new texture, but simply
				 * pulls the D3D10/11 resource in the 9Ex device */
				return _D3D9Device->CreateTexture(
					width,
					height,
					1,
					D3DUSAGE_RENDERTARGET,
					d3d9Format,
					D3DPOOL_DEFAULT,
					reinterpret_cast<IDirect3DTexture9**>(ppUnknown),
					&hSharedHandle);
			}

			/*HRESULT D3DImageEx::GetSharedHandle(IUnknown* pUnknown, HANDLE* pHandle)
			{
				HRESULT hr = S_OK;

				*pHandle = NULL;
				IDXGIResource* pSurface;

				if (FAILED(hr = pUnknown->QueryInterface(__uuidof(IDXGIResource), (void**)&pSurface)))
				{
					return hr;
				}

				hr = pSurface->GetSharedHandle(pHandle);
				pSurface->Release();

				return hr;
			}*/

			// GetSharedHandle: Direct3D 리소스에서 공유 핸들을 얻습니다.
			// GetSharedHandle: Direct3D 리소스에서 공유 핸들을 얻습니다.
			HRESULT D3DImageEx::GetSharedHandle(IUnknown* pUnknown, HANDLE* pHandle)
			{
				if (pUnknown == nullptr || pHandle == nullptr)
				{
					return E_INVALIDARG;
				}

				IDXGIResource* pDXGIResource = nullptr;
				HRESULT hr = pUnknown->QueryInterface(__uuidof(IDXGIResource), reinterpret_cast<void**>(&pDXGIResource));
				if (SUCCEEDED(hr))
				{
					hr = pDXGIResource->GetSharedHandle(pHandle);
					pDXGIResource->Release(); // 사용 후 리소스 해제
				}
				return hr;
			}


			// InitD3D9: Direct3D 9 환경을 초기화합니다.
			HRESULT D3DImageEx::InitD3D9(HWND hWnd)
			{
				HRESULT hr;

				IDirect3D9Ex* d3D9;

				Direct3DCreate9Ex(D3D_SDK_VERSION, &d3D9);

				_D3D9 = d3D9;

				if (!_D3D9)
					return E_FAIL;

				_D3D9 = d3D9;

				D3DPRESENT_PARAMETERS		d3dpp;

				ZeroMemory(&d3dpp, sizeof(d3dpp));

				d3dpp.Windowed = TRUE;
				d3dpp.SwapEffect = D3DSWAPEFFECT_DISCARD;
				d3dpp.hDeviceWindow = hWnd;
				d3dpp.PresentationInterval = D3DPRESENT_INTERVAL_IMMEDIATE;

				IDirect3DDevice9Ex* d3D9Device;

				hr = _D3D9->CreateDeviceEx(D3DADAPTER_DEFAULT,
					D3DDEVTYPE_HAL,
					hWnd,
					D3DCREATE_HARDWARE_VERTEXPROCESSING | D3DCREATE_MULTITHREADED | D3DCREATE_FPU_PRESERVE,
					&d3dpp,
					NULL,
					&d3D9Device);

				if (FAILED(hr))
				{
					return hr;
				}

				_D3D9Device = d3D9Device;

				return hr;
			}

		}
	}
}