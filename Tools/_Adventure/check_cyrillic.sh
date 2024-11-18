#!/usr/bin/sh
set -xue
(! git grep -IP -e "([a-zA-Z][а-яА-Я]|[а-яА-Я][a-zA-Z])" --and --not -e "\\\n[а-яА-Я]" "${GITHUB_BASE_REF}")
(! git ls-tree --full-tree -r "${GITHUB_BASE_REF}" | grep -P "[а-яА-Я]")
