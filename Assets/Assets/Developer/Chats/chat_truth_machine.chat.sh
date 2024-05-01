#!chat chat_truth_machine

metadata() {
  type dm
  start_type auto
  actor computer
  start_message "Play the Truth-Machine test script"
  repeatable true
}

# By default, the first actor defined in metadata() is the marked as the current actor when the script starts.
main_branch() {
  say "I am a computer."
  say "Please pick a number."

  branch 0 'Choose Zero'
  branch 1 'Choose One'

  wait_for_choices

  if branch_picked 0;
  then
    actor player
    say "I choose zero."
    
    actor computer
    say "You picked zero!"
    exit
  fi

  branch stop "Stop the script!"

  if branch_picked 1;
  then
    actor player
    say "I choose one."
    actor computer
  fi

  while branch_picked 1;
  do
    if branch_picked stop;
    then
      actor player
      say "Stop yelling at me!"
      actor computer
      say "You told me to stop computering!"
      exit
    fi
    say "You picked one!"
  done
}