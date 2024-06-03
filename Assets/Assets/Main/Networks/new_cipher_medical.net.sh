#!network new_cipher_medical

build() {
  name "New Cipher Medical"
  isp district_downtown
  
  domain newciphermedical.org
  domain www.newciphermedical.org
  
  device web "Web Server"
  device intranet "Hospital Intranet"
  device patient_records "Patient Records"
  device ed_workstation "Workstation: E.D. Triage Desk"
  
 
}