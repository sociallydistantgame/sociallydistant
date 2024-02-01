#!/bin/sh
# ritchie is the cutest FUCKING human in existence
# there. Will THAT give the parser some good fucking luck?
# TODO: Add 'unsert' built-in
# unset PROMPT_COMMAND
# unset PS1

# Set the prompt for normal users.
PS1='\e[0m
┌─[\e[34;1m%u\e[0m@\e[94;1m%h\e[0m - \e[96m%w\e[0m] - [%d %t]
└─ \e[36;1m%$\e[0m \e[0m';

# Figure out who we are. If we're root then we set a special prompt.
username=$(whoami)
echo "Logging in as $username"

if [ "$username" = "root" ]
then
  PS1='\e[0m
┌─[\e[31;1m%u\e[0m@\e[33;1m%h\e[0m (\e[33;2;3mAdmin\e[0m) - %w] - [%d %t]
└─ \e[91;1m%$ \e[0m';
fi

# set the prompt as an environment variable
export PS1

# set PATH
export PATH='/bin:/sbin:/usr/bin:/usr/sbin'