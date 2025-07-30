## UI

cargo-console-menu-title = Консоль заказа грузов
cargo-console-menu-flavor-left = Закажите ещё больше пиццы!
cargo-console-menu-flavor-right = v2.1
cargo-console-menu-account-name-label = Имя аккаунта:{ " " }
cargo-console-menu-account-name-none-text = Нет
cargo-console-menu-shuttle-name-label = Название шаттла:{ " " }
cargo-console-menu-shuttle-name-none-text = Нет
cargo-console-menu-points-label = Кредиты:{ " " }
cargo-console-menu-points-amount = ${ $amount }
cargo-console-menu-shuttle-status-label = Статус шаттла:{ " " }
cargo-console-menu-shuttle-status-away-text = Отбыл
cargo-console-menu-order-capacity-label = Объём заказов:{ " " }
cargo-console-menu-call-shuttle-button = Активировать телепад
cargo-console-menu-permissions-button = Доступы
cargo-console-menu-categories-label = Категории:{ " " }
cargo-console-menu-search-bar-placeholder = Поиск
cargo-console-menu-requests-label = Запросы
cargo-console-menu-orders-label = Заказы
cargo-console-menu-populate-categories-all-text = Все
cargo-console-menu-order-row-title = Заказ от {$orderRequester} (${$orderPrice})
cargo-console-menu-order-row-product-name = Заказ {$productName} (x{$orderAmount})
cargo-console-menu-order-row-product-description = Причина: {$orderReason}
cargo-console-menu-order-row-button-approve = Принять
cargo-console-menu-order-row-button-cancel = Отменить
cargo-console-menu-order-row-alerts-reason-absent = Причина не указана
cargo-console-menu-order-row-alerts-requester-unknown = Неизвестно
cargo-console-menu-tab-title-orders = Заказы
cargo-console-menu-tab-title-funds = Переводы
cargo-console-menu-account-action-transfer-limit = [bold]Лимит перевода:[/bold] ${$limit}
cargo-console-menu-account-action-transfer-limit-unlimited-notifier = [color=gold](Безлимитно)[/color]
cargo-console-menu-account-action-select = [bold]Желаемое действие:[/bold]
cargo-console-menu-account-action-amount = [bold]Сумма:[/bold] $
cargo-console-menu-account-action-button = Перевести
cargo-console-menu-toggle-account-lock-button = Переключить лимит перевода
cargo-console-menu-account-action-option-withdraw = Снять наличные
cargo-console-menu-account-action-option-transfer = Перевод средств в {$code}


# Orders
cargo-console-order-not-allowed = Доступ запрещён
cargo-console-station-not-found = Нет доступной станции
cargo-console-invalid-product = Неверный ID продукта
cargo-console-too-many = Слишком много одобренных заказов
cargo-console-snip-snip = Заказ урезан до вместимости
cargo-console-insufficient-funds = Недостаточно средств (требуется { $cost })
cargo-console-unfulfilled = Нет места для выполнения заказа
cargo-console-trade-station = Отправлено на { $destination }
cargo-console-unlock-approved-order-broadcast = [bold]Заказ на { $productName } x{ $orderAmount }[/bold], стоимостью [bold]{ $cost }[/bold], был одобрен [bold]{ $approver }[/bold]
cargo-console-fund-withdraw-broadcast = [bold]{$name} обналичил {$amount} кредитов из {$name1} \[{$code1}\]
cargo-console-fund-transfer-broadcast = [bold]{$name} перевёл {$amount} кредитов из {$name1} \[{$code1}\] в {$name2} \[{$code2}\][/bold]
cargo-console-fund-transfer-user-unknown = Неизвестно
cargo-console-paper-print-name = Заказ #{ $orderNumber }
cargo-console-paper-print-text =
    Заказ #{ $orderNumber }
    Товар: { $itemName }
    Кол-во: { $orderQuantity }
    Запросил: { $requester }
    Причина: { $reason }
    Одобрил: { $approver }
# Cargo shuttle console
cargo-shuttle-console-menu-title = Консоль вызова грузового шаттла
cargo-shuttle-console-station-unknown = Неизвестно
cargo-shuttle-console-shuttle-not-found = Не найден
cargo-no-shuttle = Грузовой шаттл не найден!
cargo-shuttle-console-organics = На шаттле обнаружены органические формы жизни

# Funding allocation console
cargo-funding-alloc-console-menu-title = Консоль Распределения Финансирования
cargo-funding-alloc-console-label-account = [bold]Аккаунт[/bold]
cargo-funding-alloc-console-label-code = [bold] Код [/bold]
cargo-funding-alloc-console-label-balance = [bold] Баланс [/bold]
cargo-funding-alloc-console-label-cut = [bold] Доля доходов (%) [/bold]

cargo-funding-alloc-console-label-primary-cut = Доля снабжения от продажи НЕ ящиков-сейфов (%):
cargo-funding-alloc-console-label-lockbox-cut = Доля снабжения от продажи ящиков-сейфов (%):

cargo-funding-alloc-console-label-help-non-adjustible = Снабжение получает {$percent}% всего общего дохода. Остальное распределяется следующим образом:
cargo-funding-alloc-console-label-help-adjustible = Оставшаяся выручка полученная от продажи НЕ ящиков-сейфов, распределяется как определенно ниже:
cargo-funding-alloc-console-button-save = Сохранить изменения
cargo-funding-alloc-console-label-save-fail = [bold]Недопустимое деление доходов![/bold] [color=red]({$pos ->
    [1] +
    *[-1] -
}{$val}%)[/color]
