reagent-effect-condition-guidebook-type-damage =
    { $max ->
        [2147483648] it has at least {NATURALFIXED($min, 2)} of {$type} damage
        *[other] { $min ->
                    [0] it has at most {NATURALFIXED($max, 2)} of {$type} damage
                    *[other] it has between {NATURALFIXED($min, 2)} and {NATURALFIXED($max, 2)} of {$type} damage
                 }
    }

reagent-effect-condition-guidebook-group-damage =
    { $max ->
        [2147483648] it has at least {NATURALFIXED($min, 2)} of {$type} damage.
        *[other] { $min ->
                    [0] it has at most {NATURALFIXED($max, 2)} of {$type} damage.
                    *[other] it has between {NATURALFIXED($min, 2)} and {NATURALFIXED($max, 2)} of {$type} damage
                 }
    }

reagent-effect-condition-guidebook-mob-state-condition = пациент в { $state }
reagent-effect-condition-guidebook-job-condition = должность цели — { $job }

reagent-effect-condition-guidebook-solution-temperature =
    температура раствора составляет { $max ->
        [2147483648] не менее { NATURALFIXED($min, 2) }k
       *[other]
            { $min ->
                [0] не более { NATURALFIXED($max, 2) }k
               *[other] между { NATURALFIXED($min, 2) }k и { NATURALFIXED($max, 2) }k
            }
    }

reagent-effect-condition-guidebook-organ-type =
    метаболизирующий орган { $shouldhave ->
        [true] это
       *[false] это не
    } { $name } орган

reagent-effect-condition-guidebook-has-tag =
    цель { $invert ->
        [true] не имеет
       *[false] имеет
    } метку { $tag }

reagent-effect-condition-guidebook-this-reagent = этот реагент

reagent-effect-condition-guidebook-breathing =
    цель { $isBreathing ->
        [true] дышит нормально
       *[false] задыхается
    }

reagent-effect-condition-guidebook-internals =
    цель { $usingInternals ->
        [true] использует дыхательную маску
       *[false] дышит атмосферным газом
    }
