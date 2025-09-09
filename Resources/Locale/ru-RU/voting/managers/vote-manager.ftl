ui-vote-initiator-server = Сервер

## Default.Votes

ui-vote-restart-title = Перезапуск раунда
ui-vote-restart-succeeded = Голосование о перезапуске раунда успешно.
ui-vote-restart-failed = Голосование о перезапуске раунда отклонено (требуется { TOSTRING($ratio, "P0") }).
ui-vote-restart-fail-not-enough-ghost-players = Голосование о перезапуске раунда отклонено: Минимум { $ghostPlayerRequirement }% игроков должно быть призраками чтобы запустить голосование о перезапуске. В данный момент игроков-призраков недостаточно.
ui-vote-restart-yes = Да
ui-vote-restart-no = Нет
ui-vote-restart-abstain = Воздерживаюсь
ui-vote-gamemode-title = Следующий режим игры
ui-vote-gamemode-tie = Ничья в голосовании за игровой режим! Выбирается... { $picked }
ui-vote-gamemode-win = { $winner } победил в голосовании за игровой режим!
ui-vote-map-title = Следующая карта
ui-vote-map-tie = Ничья при голосовании за карту! Выбирается... { $picked }
ui-vote-map-win = { $winner } выиграла голосование о выборе карты!
ui-vote-map-notlobby = Голосование о выборе карты действует только в предраундовом лобби!
ui-vote-map-notlobby-time = Голосование о выборе карты действует только в предраундовом лобби, когда осталось { $time }!
# Votekick votes
ui-vote-votekick-unknown-initiator = Игрок
ui-vote-votekick-unknown-target = Неизвестный игрок
ui-vote-votekick-title = { $initiator } начал голосование за кик пользователя: { $targetEntity }. Причина: { $reason }
ui-vote-votekick-yes = Да
ui-vote-votekick-no = Нет
ui-vote-votekick-abstain = Воздержаться
ui-vote-votekick-success = Голосование за кик { $target } прошло успешно. Причина кика: { $reason }
ui-vote-votekick-failure = Голосование за кик { $target } провалилось. Причина кика: { $reason }
ui-vote-votekick-not-enough-eligible = Недостаточное количество подходящих голосующих онлайн для начала голосования: { $voters }/{ $requirement }
ui-vote-votekick-server-cancelled = Голосование за кик { $target } отменено сервером.
# Votecallshuttle
ui-vote-type-callshuttle = Вызвать эвакуацию
ui-vote-call-shuttle-title = Вызвать эвакуацию
ui-vote-call-shuttle-yes = Да
ui-vote-call-shuttle-no = Нет
ui-vote-call-shuttle-succeeded = Голосование о вызове эвакуации успешно.
ui-vote-call-shuttle-failed = Голосование о вызове эвакуации отклонено требуется 75% согласных.
ui-vote-call-shuttle-cooldown = Голосование за вызов эвакуации доступно раз в 10 минут. До следующего голосования осталось { $minutes }:{ $seconds }.
