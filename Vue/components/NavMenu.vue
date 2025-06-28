<template>
<div class="top-row ps-3 navbar navbar-dark">
    <div class="container-fluid">
        <RouterLink class="navbar-brand" to="/kms"><span style="color: green;">向量知识库</span></RouterLink>
        <button title="Navigation menu" class="navbar-toggler">
            <span class="navbar-toggler-icon"></span>
        </button>
    </div>
</div>

<div class="collapse">
    <nav class="flex-column">
        <div class="nav-item">
            <RouterLink class="nav-link" to="/kms">
                <span class="oi oi-home" aria-hidden="true"></span> 知识库
            </RouterLink>
        </div>
        <div class="nav-item">
            <RouterLink class="nav-link" to="/kmsList">
                <span class="oi oi-data-transfer-download" aria-hidden="true"></span> 知识库详情
            </RouterLink>
        </div>
        <div class="nav-item">
            <RouterLink class="nav-link" to="/business">
                <span class="oi oi-bullhorn" aria-hidden="true"></span> 业务系统
            </RouterLink>
        </div>
        <div class="nav-item">
            <RouterLink class="nav-link" to="/mcp">
                <span class="oi oi-chat" aria-hidden="true"></span> Mcp
            </RouterLink>
        </div>
        <div class="nav-item">
            <RouterLink class="nav-link" to="/chat">
                <span class="oi oi-chat" aria-hidden="true"></span> 对话
            </RouterLink>
        </div>
        <div class="nav-item">
            <RouterLink class="nav-link" to="/agent">
                <span class="oi oi-flag" aria-hidden="true"></span> 智能体
            </RouterLink>
        </div>
    </nav>
</div>
</template>
<style scoped>
.navbar-toggler {
background-color: rgba(255, 255, 255, 0.1);
}

.top-row {
height: 3.5rem;
background-color: rgba(0,0,0,0.4);
}

.oi {
width: 2rem;
font-size: 1.5rem;
vertical-align: text-top;
top: -2px;
margin-left: 50px;
}

.nav-item {
padding-bottom: 0.5rem;
}

.nav-item:last-of-type {
    padding-bottom: 1rem;
}

:deep(.nav-item a){
color: #d7d7d7;
border-radius: 4px;
height: 3rem;
display: flex;
align-items: center;
line-height: 3rem;
}

:deep(.nav-item a.active){
    background-color: rgba(255,255,255,0.25);
    color: white;
}

:deep(.nav-item a:hover){
    background-color: rgba(255,255,255,0.1);
    color: white;
}

@media (min-width: 641px) {
.navbar-toggler {
    display: none;
}

.collapse {
    display: block;
}
}
</style>