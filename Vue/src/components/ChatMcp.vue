<template>
    <section class="content">
        <div style="height:510px;overflow-y: scroll;" class="box-body">       
            <div v-for=" (item, index) in props.data.chatRecord" style="clear:both; border: solid 1px #efefef;">
                <div style="background-color: #6CC2CC;border-radius: 5%;float:right;padding:5px;margin-left:10px;">用户</div>
                <div style="background-color: #efefef;border-radius: 1%;float:right;padding:5px;"  v-html="item.request"></div>
                <div style="background-color: #6CC2CC;border-radius: 5%;clear:both;float:left;padding:5px;margin-right:10px;">{{ item.model }}</div>
                <div style="background-color: #efefef;border-radius: 1%;clear:both;float:left;padding:5px;margin:10px 0px 0px 20px;" v-html="item.response"></div>
            </div>
        </div>
      <div style="height:150px;margin:10px;width:95%;">
            <div><el-Input type="textarea" v-model="message" rows="4" placeholder="请输入问题" style="resize:none;" :autocomplete="off"></el-Input></div>
            <div style="float:left;margin-top:10px;">            
            <div> 
                <el-select style="width:690px;margin-right:10px;" :multiple="true" v-model="props.data.mcp" :disabled="props.data.isDisabled" clearable>
                    <el-option v-for="item in mcpData" :value="item._id" :label="item.description"></el-option>
                </el-select>
            </div>
            </div>
            <div style="padding-top:15px;float:left;padding-left:10px;">
                <button class="btn btn-primary btn-sm" @click="Send">发送</button>
                <button class="btn btn-primary btn-sm" style="margin-left:10px;" @click="Close">关闭</button>
            </div>
        </div>
    </section>
</template>

<script setup>
import { ElSelect,ElOption,ElInput,ElMessage,ElLoading } from 'element-plus'
import { defineProps,defineEmits,ref,onMounted} from 'vue'
import { mcpPage } from '@/api/mcpApi'
import { mcp,chatRecord} from '@/api/chatApi'
const props = defineProps({
    data: {
      type: Object,
      required: true
    }
});

const mcpData = ref([]);
const message = ref('');

onMounted(async () => {
    await mcpPage(1,999).then(res=>{mcpData.value=res.data.list});
}); 

const Close = () =>
{
    if(props.data.query!=undefined)
        props.data.query();
    props.data.isShow =false;
}

const Send = async () =>
{
    if(message.value == '')
    {  
        ElMessage.error("内容不能为空");
        return;
    }

    if(props.data.mcp == undefined || props.data.mcp.length == 0)
    {  
        ElMessage.error("mcp不能为空");
        return;
    }

    let loading = ElLoading.service({ lock: true,  text: 'Loading', background: 'rgba(0, 0, 0, 0.7)' });  

    var data = new Object();
    data.mcp = [];
    props.data.mcp.forEach(element => {
        let mcp = mcpData.value.find(m=>m._id==element);
        mcp.parameters = JSON.parse(mcp.parameters);
        data.mcp.push(mcp);
    });

    data.message = message.value;
    data.chatIndex = props.data.chatIndex;

    await mcp(data).then(async res=>{
        props.data.chatIndex = res.data.chatIndex;
        await chatRecord(props.data.chatIndex).then(r=>{ props.data.chatRecord = r.data.list;});
        loading.close();
        message.value='';
        props.data.isDisabled=true;
    });
}
</script>