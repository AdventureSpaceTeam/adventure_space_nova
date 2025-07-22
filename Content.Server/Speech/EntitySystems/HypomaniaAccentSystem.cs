using System.Text.RegularExpressions;
using Content.Server.Speech.Components;
using Content.Server.Chat.Systems;
using Content.Shared.Speech;
using Robust.Shared.Random;

namespace Content.Server.Speech.EntitySystems;

public sealed class HypomaniaAccentSystem : EntitySystem
{
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<HypomaniaAccentComponent, AccentGetEvent>(OnAccent);
    }

    private void OnAccent(EntityUid uid, HypomaniaAccentComponent component, AccentGetEvent args)
    {
        var message = args.Message;

        // . => !
        message = message.Replace(".", "!");

        /// <summary>
        /// Если последний символ в сообщении не '?', добавляем в конец '!'.
        /// Эта проверка должна выполняться перед заменой '?', чтобы избежать двойного восклицания в вопросительных предложениях.
        /// <summary>
        if (message[message.Length - 1] != '?')
            message += "!";

        // ? => ?!
        message = message.Replace("?", "?!");


        // Это условие определяет, будем ли мы смеяться после фразы
        if (_random.Prob(component.LaughChance))
        {
            // Проигрываем эмоцию смеха
            _chat.TryEmoteWithChat(uid, "Laugh", ignoreActionBlocker: false);
        }

        args.Message = message;
    }
}
