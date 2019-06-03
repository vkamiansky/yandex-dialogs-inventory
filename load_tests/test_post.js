import http from "k6/http";
import { sleep, check } from "k6";

import * as config from "./config.js";
import { aliceRequest, generateCommand } from "./alice_request.js";


export let options = config.optionsSimple;
let header = { "Content-Type": "application/x-www-form-urlencoded" };


export default function()
{
    generateRequest();
    let result = http.post(config.url, aliceRequest, { headers: header });
    check(result,
        {
            "Response Time": (r) => r.timings.duration <= 1500,
            // "Correct User ID": (r) => r.body.session.user_id === aliceRequest.session.user_id,
            // "Correct Session ID": (r) => r.body.session.session_id === aliceRequest.session.session_id,
            // "Correct Message ID": (r) => r.body.session.message_id === aliceRequest.session.message_id,
        })
    console.log(`${aliceRequest.session.user_id}: ${aliceRequest.request.command}`);
    sleep(1);
}


(function initializeVU()
{
    let userId = "testUser" + Math.floor(Math.random() * options.vus * 10);
    let sessionId = userId + "session";

    aliceRequest.session.new = true;
    aliceRequest.session.message_id = 0;
    aliceRequest.session.session_id = sessionId;
    aliceRequest.session.user_id = userId;
})();

function generateRequest()
{
    aliceRequest.request.command = generateCommand();
    aliceRequest.session.new = false;
    aliceRequest.session.message_id++;
}
