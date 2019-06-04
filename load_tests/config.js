export let url = "https://alice.vsop.spb.ru/api/inventory/alice";

export let optionsSimple =
{
    vus: 10,
    duration: "10s",
};

export let optionsStages =
{
    stages:
    [
        { target: 10, duration: "10s" },
        { target: 100, duration: "50s"},
    ]
};
