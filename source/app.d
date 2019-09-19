module app;

import core.sys.windows.windows;
import core.sys.windows.dll;

__gshared HINSTANCE g_hInst;

// https://wiki.dlang.org/Win32_DLLs_in_D
extern (Windows)
BOOL DllMain(HINSTANCE hInstance, ULONG ulReason, LPVOID pvReserved)
{
    switch (ulReason)
    {
        case DLL_PROCESS_ATTACH:
            g_hInst = hInstance;
            dll_process_attach( hInstance, true );
            break;

        case DLL_PROCESS_DETACH:
            dll_process_detach( hInstance, true );
            break;

        case DLL_THREAD_ATTACH:
            dll_thread_attach( true, true );
            break;

        case DLL_THREAD_DETACH:
            dll_thread_detach( true, true );
            break;

        default:
    }
    return true;
}
