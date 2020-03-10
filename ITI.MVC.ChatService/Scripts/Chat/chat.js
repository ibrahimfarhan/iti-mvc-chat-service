let chatContainer = document.getElementById("v-pills-tabContent"),
    messageContainer = document.getElementById("messageContainer"),
    usersList = document.getElementById("v-pills-tab"),
    msgTextbox = document.getElementById("msgTextbox"),
    msgFormSendBtn = document.getElementById("msgFormSendBtn");

let targetUserId = document.querySelector(`[data-isfirstuser="true"]`);

let messageIconClasses = {
    Sent: "fas fa-check",
    Received: "fas fa-check-double",
    Read: "fas fa-check-double text-primary",
    Pending: "far fa-clock"
};

// Setup SignalR

let chatHub = $.connection.chatHub;

chatHub.client.displayMessage = (senderUserName, message, messageStatus) => {

    let icon = "";

    if (messageStatus) {

        icon = `<i class="chat-message__icon ${messageIconClasses[messageStatus]} ml-1"></i>`;
    }

    let messageHtml = `<p class="message-container__name">${senderUserName}</p>
                    <p class="chat-message">
                        ${message}
                        ${icon}
                    </p>`;

    messageContainer.innerHTML += messageHtml;
};

chatHub.client.changeMessageStatus = (messageId, newStatus) => {

    let messageIcon = document.getElementById(`messageIcon#${messageId}`);

    messageIcon.className = `chat-message__icon ${messageIconClasses[newStatus]} ml-1`;
};

chatHub.client.displayTyping = typingUserId => {

    let userNameDiv = document.querySelector(`[data-userid="${typingUserId}"]`);

    userNameDiv.innerHTML += `<small class="ml-2">typing...</small>`;
};

$.connection.hub.start();

// Test SignalR
chatHub.server.hello();

// Register events.

usersList.addEventListener("click", onUsersListClick);
msgTextbox.addEventListener("change", onMsgTextboxChange);
msgFormSendBtn.addEventListener("click", onMsgSendBtnClick);

// Handle getting chat messages with a specific user.

function onUsersListClick(e) {

    if (!e.target) {
        return;
    }

    if (e.target.classList.contains("user-in-list")) {

        targetUserId = e.target.getAttribute("data-userid");
        getMessagesPartialView(targetUserId);
    }
}

async function getMessagesPartialView(targetUserId) {

    let response = await fetch(`/Chat/Messages/?targetUserId=${targetUserId}`);
    let partialView = await response.text();

    if (response.status === 200) {

        chatContainer.innerHTML += partialView;
    }
}

//Handle message form behaviour.

function onMsgTextboxChange(e) {

    if (e.target.value.trim() !== "") {

        chatHub.server.displayTyping(targetUserId);
    }
}

function onMsgSendBtnClick() {

    chatHub.server.sendMessage(message, targetUserId);
}

