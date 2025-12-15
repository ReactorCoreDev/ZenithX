ForceShutdown = {
        Prefix = Settings.Prefix;
        Commands = {"ForceShutdown","FSD","FS","ForceSD","ForceS"};
        Args = {"Enable (true or false)","Success (true or false)"};
        Description = "Forces Shutdown";
        AdminLevel = 200; 
        Filter = true;
        Hidden = false;
        Disabled = false;
        Function = function(plr: Player, args: {string}, data)
            if not (args[1]) or not (args[2]) then
                workspace.CoreGame.Scripts.Shutdown.Forced.Value = true
                workspace.CoreGame.Scripts.Shutdown.Forced.Success.Value = true
                return
            else
                workspace.CoreGame.Scripts.Shutdown.Forced.Value = args[1]
                workspace.CoreGame.Scripts.Shutdown.Forced.Success.Value = args[2]
            end
        end
    };