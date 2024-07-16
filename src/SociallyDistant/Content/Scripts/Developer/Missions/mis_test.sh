#!mission mis_test

metadata() {
  name Test Mission
  type main
  start_type email
  giver ashe
}

email() {
  echo Hello world.
  echo This is a test.
  echo If you are reading this, "<b>this text should be bold.</b>"
}

start() {
  exit
}