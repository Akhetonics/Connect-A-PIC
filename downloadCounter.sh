#!/bin/bash

# File where the download count is stored
output_file="downloads_log.txt"
# Webhook URL for Microsoft Teams (replace with your own webhook URL)
teams_webhook_url="https://akhetde.webhook.office.com/webhookb2/72579872-087f-4ece-b775-0458bc144c2f@e3e30a58-18ff-4dfa-b3df-1e987dc47809/IncomingWebhook/0baea06a240a4d9daeb2da2293eaa787/496f2478-0986-4016-ae6d-4d79451d1486"

# Get the current timestamp
current_timestamp=$(date "+%Y-%m-%d %H:%M:%S")

# Calculate the current download count
current_download_count=$(curl -s https://api.github.com/repos/Akhetonics/Connect-A-PIC/releases | grep 'download_count' | awk -F ':' '{ sum += $2 } END { print sum }' | awk '{print $1}')

# Read the last known download count from the file
last_known_download_count=$(tail -n 1 $output_file | awk -F ": " '{print $2}')

# Check if the new download count has increased
if [ "$current_download_count" -gt "$last_known_download_count" ]; then
  # If increased, append the new download count and timestamp to the file
  echo "$current_timestamp - Download count: $current_download_count" >> $output_file
  
  # Send a message to Microsoft Teams
  curl -H "Content-Type: application/json" -d "{\"text\":\"New download detected for Connect A PIC! Total downloads: $current_download_count\"}" $teams_webhook_url

  echo "A new download was detected and a message has been sent to Microsoft Teams."
else
  echo "No new downloads detected."
fi

# Prompt to exit
echo "Press enter to exit."
#read