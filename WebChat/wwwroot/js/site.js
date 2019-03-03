console.log("Enter");
const con = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

con.on("broadcast", (user, msg, time) => {
    msg = time + "  " + user + ": " + msg;
    const li = document.createElement("li");
    li.textContent = msg;
    document.getElementById("messages").appendChild(li);
});

con.on("info", (msg, time) => {
    const li = document.createElement("li");
    li.textContent = time + "  " + msg;
    //li.textContent.fontcolor("red");
    li.style.color = 'blue';
    document.getElementById("messages").appendChild(li);
});

const sendMessage = function () {
    const msg = document.getElementById("message_input").value;
    con.invoke("SendMessage", msg);
};

const login = function () {
    const nickname = document.getElementById("nickname_field").value;
    const password = document.getElementById("password_field").value;

    const xhr = new XMLHttpRequest();

    xhr.onreadystatechange = function() {
        if (xhr.readyState != 4 || xhr.status != 200) return;
        location.href = "https://localhost:5001/";
    };
    
    const signInRequest = JSON.stringify({
        name: nickname,
        password: password,
    });
    xhr.open('POST', 'https://localhost:5001/login', true);
    xhr.setRequestHeader('Content-type', 'application/json; charset=utf-8');
    xhr.withCredentials = true;
    xhr.send(signInRequest);
};

const join = function () {
    const roomName = document.getElementById("room_name").value;
    const roomPassword = document.getElementById("room_password").value;

    console.log("join");
    const xhr = new XMLHttpRequest();

    flag = true;
    
    xhr.onreadystatechange = function() {
        if (!flag || xhr.readyState != 4 || xhr.status != 200) return;
        flag = false;
        con.start()
            .then(() => {
                console.log("connection established");
                document.getElementById("div1").innerHTML = 
                    "    <div class=\"center2 text-center\">\n" +
                    "        <div class=\"container-fluid bg-light h-100\">\n" +
                    "            <ul id='messages' class=\"list-group list-group-flush h-100\">\n" +
                    "            </ul>\n" +
                    "        </div>\n" +
                    "        <input type=\"text\" id=\"message_input\" placeholder=\"Enter message\"/>\n" +
                    "        <button type=\"button\" class=\"btn-success\" onclick=\"sendMessage()\">Send</button>\n" +
                    "    </div>";
            })
            .catch(err => console.error(err.toString()));
    };
    
    const request = JSON.stringify({
        name: roomName,
        password: roomPassword,
    });    

    
    xhr.open('POST', 'https://localhost:5001/chat/join', true);
    xhr.setRequestHeader('Content-type', 'application/json; charset=utf-8');
    xhr.withCredentials = true;
    xhr.send(request);
};