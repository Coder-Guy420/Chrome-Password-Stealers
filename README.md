# Chrome-Passwod-Stealer

Stealer.py
----------
This program will steal people's saved chrome passwords and decrypt them, then save the data in a text file where the format goes like this,

URL: [url of the website]
Username: [username]
Password: [password]


Discord Webhook Exfiltration
----------------------------
This does the same thing as stealer.py, but it sends the passwords to a discord webhook. Keep in mind that you have to replace 'YOUR WEBHOOK URL' with the url of you webhook.

SMS Exfiltration
----------------
This does the same thing as stealer.py, but it sends the passwords to a sms phone number. You have to repalce 'TWILIO_ACCOUNT_SID' with your Twilio account SID,
'TWILIO_AUTH_TOKEN' with your Twilio account authentication token, 'TWILIO_PHONE_NUMBER' with your Twilio phone number, and 'TARGET_PHONE_NUMBER' with the phone number to which you want to send the SMS message to.

Remote Server Exfiltration
--------------------------
This does the same thing as stealer.py, but it sends the passwords to a remote server. You have to replace 'YOUR REMOTE SERVER' with the url of you remote server.
