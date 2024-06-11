#!npc chr_local_news

profile() {
  username newciphertoday
  displayname New Cipher Today
  bio Breaking news and local stories that matter most to the citizens of New Cipher.
  pronoun enby
  
  # NPC attributes.
  # - integral: player always sees this user's posts in their timeline
  # - verified: blue checkmark
  # - scripted: no randomly-generated posts
  attributes integral verified scripted
}

feed() {
  # TODO
}