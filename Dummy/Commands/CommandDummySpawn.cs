using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dummy.API;
using Dummy.Models;
using Dummy.Users;
using Microsoft.Extensions.Configuration;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Players;
using Steamworks;
using Dummy.Services;

namespace Dummy.Commands
{
    [Command("spawn")]
    [CommandParent(typeof(CommandDummy))]
    public class CommandDummySpawn : Command
    {
        private readonly DummyUserProvider m_DummyUserProvider;
        private readonly IConfiguration m_Configuration;

        public CommandDummySpawn(IServiceProvider serviceProvider, DummyUserProvider dummyUserProvider, IConfiguration configuration)
            : base(serviceProvider)
        {
            m_DummyUserProvider = dummyUserProvider;
            m_Configuration = configuration;
        }

protected override async Task OnExecuteAsync()
{
    var name = CommandDummy.s_NameArgument.GetArgument(Context.Parameters);
    if (!string.IsNullOrEmpty(name))
    {
        m_Configuration.Get<Configuration>().Default.CharacterName = name!;
        m_Configuration.Get<Configuration>().Default.NickName = name!;
        m_Configuration.Get<Configuration>().Default.PlayerName = name!;
    }

    // Get a valid Steam ID from the file
    var steamId = m_DummyUserProvider.GetNextSteamId();

    // Use the retrieved Steam ID for the dummy creation
    await m_DummyUserProvider.AddDummyAsync(steamId, new HashSet<CSteamID>());
}
    }
}
