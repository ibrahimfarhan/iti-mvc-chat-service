// DOM elements

let chatContainer = document.getElementById("v-pills-tabContent"),
    messageContainer = document.getElementById("messageContainer"),
    usersList = document.getElementById("v-pills-tab"),
    msgTextbox = document.getElementById("msgTextbox"),
    msgForm = document.getElementById("msgForm"),
    msgFormSendBtn = document.getElementById("msgFormSendBtn");

// Global variables

let targetUserId = document.querySelector(`[data-isfirstuser="true"]`).getAttribute("data-userid"),
    typingIsDisplayed = false;

let messageIconClasses = {
    Sent: "fas fa-check",
    Received: "fas fa-check-double",
    Read: "fas fa-check-double text-primary",
    Pending: "far fa-clock"
};

// Setup SignalR

let chatHub = $.connection.chatHub;

chatHub.client.displayIncomingMessage = (senderUserName, message) => {

    let messageHtml = `<div class="message-container__line">
                            <p class="message-container__name">${senderUserName}</p>
                            <p class="chat-message">
                                ${message}
                            </p>
                        </div>`;

    messageContainer.innerHTML += messageHtml;

    messageContainer.scrollTop = messageContainer.scrollHeight;
};

chatHub.client.displayOutgoingMessage = (senderUserName, message, messageStatus, messageId) => {

    let icon = "";

    if (messageStatus) {

        icon = `<i id="messageIcon#${messageId}" class="chat-message__icon ${messageIconClasses[messageStatus]} ml-1"></i>`;
    }

    let messageHtml = `<div class="message-container__line">
                            <p class="message-container__name text-primary">${senderUserName}</p>
                            <p class="chat-message">
                                ${message}
                                ${icon}
                            </p>
                        </div>`;

    messageContainer.innerHTML += messageHtml;

    messageContainer.scrollTop = messageContainer.scrollHeight;
};

chatHub.client.changeMessageStatus = (messageId, newStatus) => {

    let messageIcon = document.getElementById(`messageIcon#${messageId}`);

    messageIcon.className = `chat-message__icon ${messageIconClasses[newStatus]} ml-1`;
};

chatHub.client.displayTyping = typingUserId => {

    let userNameDiv = document.querySelector(`[data-userid="${typingUserId}"]`);

    if (!typingIsDisplayed) {

        let typingDiv = userNameDiv.querySelector(".typing");
        typingDiv.style.display = "block";
        typingIsDisplayed = true;
    }
};

chatHub.client.removeTyping = typingUserId => {

    let userNameDiv = document.querySelector(`[data-userid="${typingUserId}"]`);

    if (typingIsDisplayed) {

        let typingDiv = userNameDiv.querySelector(".typing");
        typingDiv.style.display = "none";
        typingIsDisplayed = false;
    }
};

// Handle getting chat messages with a specific user.

function onUsersListClick(e) {

    if (!e.target) {
        return;
    }

    if (e.target.classList.contains("user-in-list")) {

        targetUserId = e.target.getAttribute("data-userid").getAttribute("data-userid");
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

// Handle message form behaviour.

function onMsgFormSubmit(e) {

    e.preventDefault();
    msgTextbox.value = "";
    chatHub.server.removeTyping(targetUserId);
}

function onMsgTextboxInput(e) {

    if (e.target.value.trim() !== "") {

        chatHub.server.displayTyping(targetUserId);
    }

    else {
        chatHub.server.removeTyping(targetUserId);
    }
}

function onMsgTextboxFocus() {

    chatHub.server.markReceivedMessagesAsRead();
}

function onMsgSendBtnClick() {

    let message = msgTextbox.value;

    if (message.trim() === "") {
        return;
    }

    chatHub.server.sendMessage(targetUserId, message);
}

// Handle closing signalR connection.
function onWindowClosing() {

    chatHub.server.removeCurrentConnection();
    $.connection.hub.stop();
}

function init() {

    // Start signalR.
    $.connection.hub.start().done();

    // Register events.
    usersList.addEventListener("click", onUsersListClick);
    msgTextbox.addEventListener("input", onMsgTextboxInput);
    msgTextbox.addEventListener("focus", onMsgTextboxFocus);
    msgForm.addEventListener("submit", onMsgFormSubmit);
    msgFormSendBtn.addEventListener("click", onMsgSendBtnClick);
    window.addEventListener("beforeunload", onWindowClosing);

    // Display latest messages.
    messageContainer.scrollTop = messageContainer.scrollHeight;
}

init();