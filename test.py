import pika
import json
import os

# Подключение
url = os.getenv("ConnectionStrings__RabbitMq", "")
params = pika.URLParameters(url)
connection = pika.BlockingConnection(params)
channel = connection.channel() 

# Объявляем очередь (если её нет — создаст)
channel.queue_declare(queue='notifications', durable=False)

# Твоё сообщение
message = {
    "type": "friend_invite",
    "senderName": "Ника Устименко",
    "recipientEmail": "test@mail.ru",
    "action": "accept"
}
message_body = json.dumps(message)

# Отправляем сообщение в очередь
channel.basic_publish(
    exchange='',  # пустой exchange — прямая отправка в очередь
    routing_key='notifications',
    body=message_body,
    properties=pika.BasicProperties(
        delivery_mode=2  # делаем сообщение устойчивым (persistent)
    )
)

print("✅ Message sent to 'notifications' queue")

connection.close()
