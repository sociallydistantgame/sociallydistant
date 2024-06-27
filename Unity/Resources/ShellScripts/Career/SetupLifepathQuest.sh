#!WorldScript

# defineactor ritchier Ritchie
# defineactor ash "Ash Ketchum" 

function say_hi_to_ritchie() {
  actor player
  say "Hi $1" ritchie
  
  exit 0
}

function say_hi_to_ash() {
  actor player
  say "Hi $1" ash
  
  exit 0
}


function branch_test() {
  actor ritchie
  say "Hello, $1!" ash
  
  actor ash
  say "Hello, $1!" ritchie
  
  choice ritchie say_hi_to_ritchie "Say hello to $1."
  choice ash say_hi_to_ash "Say hello to $1."
}
exit 0