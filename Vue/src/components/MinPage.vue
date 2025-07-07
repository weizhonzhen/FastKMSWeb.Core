<template>
    <div class="pull-right" style="margin-top:-25px;">
        <nav aria-label="Page navigation">
            <ul class="pagination">
                <li class="active">
                    <span>共{{props.data.totalRecord}}条</span>
                    <span>{{props.data.pageId}}/{{props.data.totalPage}}页</span>
                </li>
                <li @click="queryPageId(1)"><a href="#" aria-label="Previous"><span aria-hidden="true">首页</span></a></li>
                <li @click="queryPre"><a href="#" aria-label="Previous"><span aria-hidden="true">上一页</span></a></li>
                <li @click="queryNext"><a href="#" aria-label="Next"><span aria-hidden="true">下一页</span></a></li>
                <li @click="queryLast"><a href="#" aria-label="Previous"><span aria-hidden="true">末页</span></a></li>
            </ul>
        </nav>
    </div>
</template>

<script setup>
import { defineProps ,inject} from 'vue'

const props = defineProps({
    data: {
      type: Object,
      required: true,
      default: () => ({
        totalRecord: 0,
        totalPage: 0,
        pageId: 1,
        pageSize: 10
      })
    },
    list:{
      type: Object,
      required: true,
    }
});

const pageEvent = inject("pageEvent");
const emit = defineEmits(['update:page'],['update:list']);

 const queryPageId = (i) => 
 {
      props.data.pageId = i;
      if (props.data.pageId < 1)
           props.data.pageId = 1;
      if (props.data.pageId > props.data.totalPage)
          props.data.pageId = props.data.totalPage;

      query();
    }

   const queryNext = ()=>
    {    
       if (props.data.pageId >= props.data.totalPage)
           return;

      props.data.pageId++;
      query();
    }

    const queryPre = () =>
    {
      if (props.data.pageId <= 1)
          return;
      props.data.pageId--;

      if (props.data.pageId < 1)
          props.data.pageId = 1;
    
      query();
    }

    const queryLast = ()=>
    {
      props.data.pageId = props.data.totalPage;
      query();
    }

    const query = ()=>
    {
      emit('update:page', {
          totalRecord: props.data.totalRecord,
          totalPage: props.data.totalPage,
          pageId:props.data.pageId,
          pageSize: props.data.pageSize,
      });
      
      pageEvent(props.data);
    }    
</script>