import http from "k6/http";
import { sleep, check } from "k6";

import * as config from "./config.js";


export let options = config.optionsStages;


export default function()
{
    let result = http.get(config.url);
    check(result, 
    {
        "Response OK": (r) => r.status == 200,
        "Time": (r) => r.timings.duration < 150,
    });
    sleep(1);
};
