<template>
    <div class="pull-right" style="margin-top:-25px;">
        <nav aria-label="Page navigation">
            <ul class="pagination">
                <li class="active">
                    <span>共{{props.page.totalRecord}}条</span>
                    <span>{{props.page.pageId}}/{{props.page.totalPage}}页</span>
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
    page: {
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
      props.page.pageId = i;
      if (props.page.pageId < 1)
           props.page.pageId = 1;
      if (props.page.pageId > props.page.totalPage)
          props.page.pageId = props.page.totalPage;

      query();
    }

   const queryNext = ()=>
    {    
       if (props.page.pageId >= props.page.totalPage)
           return;

      props.page.pageId++;
      query();
    }

    const queryPre = () =>
    {
      if (props.page.pageId <= 1)
          return;
      props.page.pageId--;

      if (props.page.pageId < 1)
          props.page.pageId = 1;
    
      query();
    }

    const queryLast = ()=>
    {
      props.page.pageId = props.page.totalPage;
      query();
    }

    const query = ()=>
    {
      emit('update:page', {
          totalRecord: props.page.totalRecord,
          totalPage: props.page.totalPage,
          pageId:props.page.pageId,
          pageSize: props.page.pageSize,
      });
      
      pageEvent(props.page);
    }    
</script>