const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

connection.on("broadcast message", (msg) => {
    console.log(msg);
});

connection.start().catch(err => console.error(err.toString()));

const sendMessage = function () {
    const msg = document.getElementById("message-field").value;
    connection.invoke("SendMessage", msg);
};