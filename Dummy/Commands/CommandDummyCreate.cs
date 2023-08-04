extern alias JetBrainsAnnotations;
using System;
using System.Threading.Tasks;
using Dummy.API;
using Dummy.Extensions;
using Dummy.Models;
using JetBrainsAnnotations::JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Users;

namespace Dummy.Commands
{
    [Command("create")]
    [CommandDescription("Creates a dummy")]
    [CommandActor(typeof(UnturnedUser))]
    [CommandParent(typeof(CommandDummy))]
    [UsedImplicitly]
    public class CommandDummyCreate : Command
    {
        private readonly IDummyProvider m_DummyProvider;
        private readonly IStringLocalizer m_StringLocalizer;
        private readonly IConfiguration m_Configuration;
        private readonly DummyUserProvider m_DummyUserProvider;

        public CommandDummyCreate(IServiceProvider serviceProvider, IDummyProvider dummyProvider,
            IStringLocalizer stringLocalizer, IConfiguration configuration, DummyUserProvider dummyUserProvider) : base(serviceProvider)
        {
            m_DummyProvider = dummyProvider;
            m_StringLocalizer = stringLocalizer;
            m_Configuration = configuration;
            m_DummyUserProvider = dummyUserProvider;
        }

        protected override async Task OnExecuteAsync()
        {
            var user = (UnturnedUser)Context.Actor;

            ConfigurationSettings? settings = null;
            var name = CommandDummy.s_NameArgument.GetArgument(Context.Parameters);
            if (!string.IsNullOrEmpty(name))
            {
                settings = m_Configuration.GetSection("default").Get<ConfigurationSettings>();

                settings.CharacterName = name!;
                settings.NickName = name!;
                settings.PlayerName = name!;
            }

            // Get a valid Steam ID from the file
            var steamId = m_DummyUserProvider.GetNextSteamId();

            var playerDummy = await m_DummyProvider.AddDummyByParameters(steamId, new() { user.SteamId }, settings);
            await playerDummy.TeleportToPlayerAsync(user);

            await PrintAsync(m_StringLocalizer["commands:general:create", new { Id = playerDummy.SteamId }]);
        }
    }
}
