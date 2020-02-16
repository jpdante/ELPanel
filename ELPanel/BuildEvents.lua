print("[LuaBuildEvents] Starting ELPanel PostBuild\n")
require("lua.io")

if args[2] == "Debug" then
    print("[LuaBuildEvents] Running in Debug Mode\n")
	File.copy([[C:\Users\jpdante\Documents\GitHub\ELPanel\ELPanel\bin\Debug\netcoreapp3.1\ELPanel.plugin.dll]], [[C:\Users\jpdante\Documents\GitHub\HtcSharp\HtcSharp.Server\bin\Debug\plugins\ELPanel.plugin.dll]], true)
	File.copy([[C:\Users\jpdante\Documents\GitHub\ELPanel\ELPanel\bin\Debug\netcoreapp3.1\ELPanel.plugin.pdb]], [[C:\Users\jpdante\Documents\GitHub\HtcSharp\HtcSharp.Server\bin\Debug\plugins\ELPanel.plugin.pdb]], true)
elseif args[2] == "Release" then
    print("[LuaBuildEvents] Running in Release Mode\n")
	File.copy([[C:\Users\jpdante\Documents\GitHub\ELPanel\ELPanel\bin\Release\netcoreapp3.1\ELPanel.plugin.dll]], [[C:\Users\jpdante\Documents\GitHub\HtcSharp\HtcSharp.Server\bin\Release\plugins\ELPanel.plugin.dll]], true)
end
print("[LuaBuildEvents] Finishing ELPanel PostBuild\n")