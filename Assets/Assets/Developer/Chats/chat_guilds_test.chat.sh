#!chat chat_guilds_test

metadata() {
    type guild
    start_type auto
    actor brodie
    actor vaxry
    actor skelly
    start_message "Say hi"
    guild gld_hyper_land
    channel home
    repeatable true
}

main_branch() {
    actor player
    say "Hi!"
    
    actor brodie
    say "$(mention player) Hello!"
    
    actor vaxry
    say "hi $(mention player)"
    
    actor skelly
    say "I\'m a skeleton"
}