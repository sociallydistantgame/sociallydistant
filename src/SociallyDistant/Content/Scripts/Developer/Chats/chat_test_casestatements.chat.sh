#!chat chat_test_casestatements

metadata() {
    type dm
    start_type auto
    actor ritchie
    start_message "Case Statements Test"
    repeatable true
}

main_branch() {
    say "Please enter a number."
    
    branch 1 One
    branch 2 Two
    branch 3 Three
    
    case "$(wait_for_choices)" in
        1)
            actor player
            say "I choose one"
            ;;
        2)
            actor player
            say "I choose two"
            ;;
        3)
            actor player
            say "I choose three"
            ;;
        *)
            actor player
            say "I choose none of them"
            ;;
    esac
    
    actor ritchie
    say "Okay."
}